using AutoMapper;
using MediatR;
using System.Diagnostics;
using System.Text.Json;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Dto;
using TicketProject.Models.Dto.TicketService;
using TicketProject.Services.Interfaces;
using static TicketProject.Models.Enums;

namespace TicketProject.Commands.Handlers
{
    public class BuyTicketHandlerAsync : IRequestHandler<BuyTicketCommand, bool>
    {
        private readonly IRedisService _redisService;
        private readonly IRetryService _retryService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ITicketService _ticketService;
        private readonly IErrorHandler<BuyTicketHandlerAsync> _errorHandler;
        private readonly ICampaignReadDao _campaignReadDao;
        private readonly IMapper _mapper;
        private readonly string _exchange = "ticket_exchange";
        private readonly string _routekey = "ticket_routekey";
        private string _getCampaignKey = string.Empty;

        public BuyTicketHandlerAsync(IRedisService redisService, IRetryService retryService,
            ITicketService ticketService, IRabbitMQService rabbitMQService,
            IErrorHandler<BuyTicketHandlerAsync> errorHandler,
            IMapper mapper, ICampaignReadDao campaignReadDao)
        {
            _redisService = redisService;
            _retryService = retryService;
            _ticketService = ticketService;
            _rabbitMQService = rabbitMQService;
            _errorHandler = errorHandler;
            _mapper = mapper;
            _campaignReadDao = campaignReadDao;
        }

        public async Task<bool> Handle(BuyTicketCommand request, CancellationToken cancellationToken)
        {
            var inserToQueueLockKey = $"Campaign:lock:{request.CampaignId}";
            _getCampaignKey = $"Campaign:Id:{request.CampaignId}";
            var lockTokenSource = new CancellationTokenSource();
            try
            {
                // 將沒有競爭隱患的邏輯移至取鎖之前 避免佔用鎖
                var campaign = await GetCampaignCache() ?? await GetCampaignFromDb(request.CampaignId);
                var ticketContents = campaign.TicketContents.ToList();
                var tickeType_QuantityDict = request.BuyList.ToDictionary(i => i.TicketType, i => i.Quantity);
                var ticketContentDict = ticketContents.ToDictionary(t => t.TypeName);

                if (!CheckTicketCount(ticketContentDict, tickeType_QuantityDict))
                    return false;

                var order = new OrderDto
                {
                    UserId = int.Parse(request.UserId),
                    OrderDate = DateTime.UtcNow
                };
                var updateTicketContentDtos = new List<UpdateTicketContentDto>();

                var stopwatch1 = Stopwatch.StartNew();
                var insertToQueueLock = await TryGetLockWithRetryAsync(inserToQueueLockKey, TimeSpan.FromMilliseconds(100), 100, 10);
                stopwatch1.Stop();
                _errorHandler.Debug($"TryGetLock: User={request.UserId} Time={stopwatch1.ElapsedMilliseconds}ms");

                if (insertToQueueLock)
                {
                    var stopwatch3 = Stopwatch.StartNew();
                    var extensionLockTask = _redisService.StartLockExtensionTaskAsync(inserToQueueLockKey, TimeSpan.FromMilliseconds(50), 10, lockTokenSource);
                    try
                    {
                        // 取得鎖後重取快取檢查 避免進入重試區塊後 上個取的鎖的執行序已變更數量
                        if (!HandleBuyList(request, ticketContents, ref order, ref updateTicketContentDtos))
                            return false;

                        // 寫入Redis的快取需統一格式
                        var campaigns = new[] { await GetCampaignCache() };

                        // 只更新Redis的可用票數 不更新DB以提升吞吐量，持久化更新由Service批量處理
                        await _redisService.SetJsonCacheAsync(_getCampaignKey, campaigns, TimeSpan.FromMinutes(5));
                        lockTokenSource.Cancel();
                    }
                    finally
                    {
                        await _redisService.ReleaseLockAsync(inserToQueueLockKey);
                        stopwatch3.Stop();
                        _errorHandler.Debug($"ReleaseLock: User={request.UserId} Time={stopwatch3.ElapsedMilliseconds}ms");
                    }

                    var insertDataDto = new InsertDataDto
                    {
                        Order = _mapper.Map<OrderDto>(order),
                        UpdateTicketContentDtos = updateTicketContentDtos
                    };

                    // 將資料整理插入Queue待批次處理
                    _rabbitMQService.PublishMessage(_exchange, JsonSerializer.Serialize(_mapper.Map<InsertDataDto>(insertDataDto)), _routekey);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        private async Task<CampaignDto?> GetCampaignCache()
        {
            var campaign = await _redisService.GetJsonCacheAsync<ICollection<CampaignDto>>(_getCampaignKey);
            return campaign?.FirstOrDefault();
        }

        private async Task<CampaignDto> GetCampaignFromDb(int campaignId)
        {
            var getDbLockKey = $"Campaign:DbLock:{campaignId}";
            var getDbLock = await _redisService.TryGetLockAsync(getDbLockKey, TimeSpan.FromMinutes(3));
            if (getDbLock)
            {
                return (await _campaignReadDao.GetCampaignAsync(i => i.CampaignId == campaignId, _getCampaignKey)).First();
            }
            else
            {
                return await RetryGetCampaignCacheAsync();
            }
        }

        private async Task<CampaignDto> RetryGetCampaignCacheAsync()
        {
            CampaignDto? campaign = null;
            await _retryService.RetryAsync(async () =>
            {
                campaign = await GetCampaignCache();
                return campaign != null;
            }, 100, 10);
            return campaign!;
        }

        private static bool CheckTicketCount(Dictionary<TicketType, TicketContentDto> ticketContentDict, Dictionary<TicketType, int> tickeType_QuantityDict)
        {
            return tickeType_QuantityDict.All(item =>
                ticketContentDict.TryGetValue(item.Key, out var ticketContent) && ticketContent.QuantityAvailable >= item.Value
            );
        }

        private static bool HandleBuyList(BuyTicketCommand request, List<TicketContentDto> ticketContents,
                ref OrderDto order, ref List<UpdateTicketContentDto> updateTicketContentDtos)
        {
            foreach (var item in request.BuyList)
            {
                var ticketContent = ticketContents.First(t => t.TypeName == item.TicketType);

                ticketContent.QuantityAvailable -= item.Quantity;
                ticketContent.QuantitySold += item.Quantity;
                ticketContent.UpdateAt = DateTime.UtcNow;

                // 再次檢查 避免進入前的數量判斷延遲
                if (ticketContent.QuantityAvailable < 0)
                    return false;

                updateTicketContentDtos.Add(new UpdateTicketContentDto
                {
                    TicketContentDto = ticketContent,
                    Quantity = item.Quantity
                });

                order.TotalAmount += ticketContent.Price * item.Quantity;

                order.OrderItems.Add(new OrderItemDto
                {
                    TicketContentId = ticketContent.TicketContentId,
                    Quantity = item.Quantity,
                    Ticket = new TicketDto
                    {
                        UserId = Convert.ToInt32(request.UserId),
                        TicketContentId = ticketContent.TicketContentId,
                    }
                });
            }
            return true;
        }

        private async Task<bool> TryGetLockWithRetryAsync(string lockKey, TimeSpan lockTimeout, int retryCount, int retryDelay)
        {
            var insertToQueueLock = false;
            await _retryService.RetryAsync(async () =>
            {
                insertToQueueLock = await _redisService.TryGetLockAsync(lockKey, lockTimeout);
                return insertToQueueLock;
            }, retryCount, retryDelay);
            return insertToQueueLock;
        }
    }
}
