using TicketProject.Services.Interfaces;
using System.Text.Json;
using RabbitMQ.Client;
using TicketProject.DAL.Interfaces;
using RabbitMQ.Client.Events;
using System.Text;
using AutoMapper;
using TicketProject.Models.Entity;
using TicketProject.Models.Dto.TicketService;

namespace TicketProject.Services.Implement
{
    public class TicketService : ITicketService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IRetryService _retryService;
        private readonly IRedisService _redisService;
        private readonly IErrorHandler<TicketService> _errorHandler;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        private readonly int _batchSize = 100;
        private readonly TimeSpan _batchInterval = TimeSpan.FromSeconds(3);
        private List<InsertDataDto> _messages = [];
        private readonly string _insetTicketQueue = "ticket_queue";
        private int _debugSuccessCount = 0;
        private int _debugCount = 0;

        private readonly SemaphoreSlim _timerLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _messagesLock = new SemaphoreSlim(1, 1);

        public TicketService(IRabbitMQService rabbitMQService, IRedisService redisService, IRetryService retryService,
            IErrorHandler<TicketService> errorHandler, IMapper mapper, IServiceProvider serviceProvider)
        {
            _rabbitMQService = rabbitMQService;
            _redisService = redisService;
            _retryService = retryService;
            _errorHandler = errorHandler;
            _mapper = mapper;
            _serviceProvider = serviceProvider;

            StartInsertConsume(_insetTicketQueue);
        }

        public void CleanMessagesForDebug()
        {
            _messages.Clear();
        }

        private void StartInsertConsume(string queue)
        {
            Func<IModel, BasicDeliverEventArgs, Task> func = async (channel, ea) =>
            {
                InsertDataDto insertDataDto = new();
                try
                {
                    var body = ea.Body.ToArray();
                    var text = Encoding.UTF8.GetString(body);
                    insertDataDto = JsonSerializer.Deserialize<InsertDataDto>(text)!;

                    // 嘗試在當前實例新增訊息 失敗則將訊息重新丟回隊列
                    if(await _messagesLock.WaitAsync(500))
                    {
                        try
                        {
                            _messages.Add(insertDataDto);
                        }
                        finally
                        {
                            _messagesLock.Release();
                        }
                    }
                    else
                    {
                        channel.BasicNack(ea.DeliveryTag, false, true);
                        return;
                    }

                    CheckQueueTimer();
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    _errorHandler.HandleError(e);
                    channel.BasicNack(ea.DeliveryTag, false, true);
                    // 確保訊息不會丟失 
                    while (true)
                    {
                        if (await _messagesLock.WaitAsync(0))
                        {
                            try
                            {
                                _messages.Remove(insertDataDto);
                                break;
                            }
                            finally
                            {
                                _messagesLock.Release();
                            }
                        }
                        await RandomDelay(100);
                    }
                    throw;
                }
            };
            _rabbitMQService.AddConsumerAsync(queue, func, false);
        }

        private async void CheckQueueTimer()
        {
            // 確保只有一個執行緒進入 且取得鎖失敗不等待 也不需非同步鎖 實例需要有自己的Timer
            if (await _timerLock.WaitAsync(0))
            {
                try
                {
                    _errorHandler.Debug("CheckQueueTimer Start");
                    if (_messages.Count < _batchSize)
                    {
                        await Task.Delay(_batchInterval);
                    }

                    // 批量更新持久性資料需要取得非同步鎖 多個實例不可同時執行
                    var batchInsertLockKey = "TicketService:BatchInsert";
                    var batchInsertLock = false;
                    batchInsertLock = await _retryService.RetryAsync(async () =>
                    {
                        return await _redisService.TryGetLockAsync(batchInsertLockKey, TimeSpan.FromSeconds(5));
                    }, 3000, 5);
                    if (batchInsertLock)
                    {
                        var token = new CancellationTokenSource();
                        var lockTask = _redisService.StartLockExtensionTaskAsync(batchInsertLockKey,
                                        TimeSpan.FromSeconds(3), 3, token);
                        try
                        {
                            await BatchInsert();
                            token.Cancel();
                        }
                        finally
                        {
                            await _redisService.ReleaseLockAsync(batchInsertLockKey);
                        }
                    }
                    _errorHandler.Debug("CheckQueueTimer End");
                }
                catch (Exception e)
                {
                    _errorHandler.HandleError(e);
                    throw;
                }
                finally
                {
                    _timerLock.Release();
                }
            }
        }

        private async Task BatchInsert()
        {
            try
            {
                if (_messages.Count > 0)
                {
                    while (true)
                    {
                        if (await _messagesLock.WaitAsync(1000))
                        {
                            var scope = _serviceProvider.CreateScope();
                            try
                            {
                                var orderWriteDao = scope.ServiceProvider.GetRequiredService<IOrderWriteDao>();
                                var ticketWriteDao = scope.ServiceProvider.GetRequiredService<ITicketContentWriteDao>();
                                var ticketReadDao = scope.ServiceProvider.GetRequiredService<ITicketContentReadDao>();

                                var orders = _messages.Select(x => _mapper.Map<Order>(x.Order)).ToList();
                                var ticketContentId_QuantityDict = new Dictionary<int, int>();

                                foreach (var message in _messages)
                                {
                                    foreach (var item in message.UpdateTicketContentDtos)
                                    {
                                        if (ticketContentId_QuantityDict.TryGetValue(item.TicketContentDto.TicketContentId, out var existingQuantity))
                                            ticketContentId_QuantityDict[item.TicketContentDto.TicketContentId] = existingQuantity + item.Quantity;
                                        else
                                            ticketContentId_QuantityDict[item.TicketContentDto.TicketContentId] = item.Quantity;
                                    }
                                }

                                var ticketContentIds = ticketContentId_QuantityDict.Keys.ToList();
                                var ticketContents = (await ticketReadDao.GetTicketContentsAsync(t => ticketContentIds.Contains(t.TicketContentId)))!.ToList();
                                ticketContents.ForEach(t =>
                                {
                                    t.QuantityAvailable -= ticketContentId_QuantityDict[t.TicketContentId];
                                    t.QuantitySold += ticketContentId_QuantityDict[t.TicketContentId];
                                });

                                await orderWriteDao.CreateOrdersAsync(orders);
                                await ticketWriteDao.UpdateTicketsContentsAsync(ticketContents);
                                _messages.RemoveRange(0, _messages.Count);
                                break;
                            }
                            finally
                            {
                                _messagesLock.Release();
                                scope.Dispose();
                            }
                        }
                        await RandomDelay(100);
                    }
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 隨機延遲n~n+1/4毫秒 (無視小數)
        /// </summary>
        /// <param name="delayMs">隨機的底限 毫秒</param>
        /// <returns></returns>
        private Task RandomDelay(int delayMs)
        {
            var random = new Random();
            var delay = random.Next(delayMs, delayMs / 4);
            return Task.Delay(delay);
        }
    }
}
