using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IErrorHandler<RabbitMQService> _errorHandler;
        // 無序的集合，設計用於在多線程環境中進行高效的數據存取
        private readonly ConcurrentBag<IModel> _channels = [];
        private readonly int _maxSize;
        private int _currentSize;
        private readonly SemaphoreSlim _producerSemaphore;

        public RabbitMQService(IConnection connection, IErrorHandler<RabbitMQService> errorHandler,
            ConcurrentBag<IModel> channels, int maxSize)
        {
            _connection = connection;
            _errorHandler = errorHandler;
            _channels = channels;
            _maxSize = maxSize;
            _producerSemaphore = new SemaphoreSlim(maxSize, maxSize);
        }

        public async void CleanQueueForDebug()
        {
            IModel? channel = null;
            try
            {
                channel = await GetChannel();
                channel.QueuePurge("ticket_queue");
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                ReturnChannel(channel);
            }
        }

        private async Task<IModel> GetChannel()
        {
            await _producerSemaphore.WaitAsync();
            try
            {
                if (_channels.TryTake(out var channel))
                {
                    if (channel.IsOpen)
                        return channel;
                    else
                    {
                        // Channel 已關閉，創建新 Channel
                        Interlocked.Decrement(ref _currentSize);
                        return await GetChannel();
                    }
                }
                else
                {
                    if (_currentSize < _maxSize)
                    {
                        // 用於在多線程環境中安全地遞增一個整數值
                        Interlocked.Increment(ref _currentSize);
                        return _connection.CreateModel();
                    }
                    else
                        throw new InvalidOperationException("Producer channel pool exhausted");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        private void ReturnChannel(IModel? channel)
        {
            if (channel != null && channel.IsOpen)
            {
                _channels.Add(channel);
            }
            else
            {
                // Channel 已關閉，減少當前大小
                Interlocked.Decrement(ref _currentSize);
            }
            _producerSemaphore.Release();
        }

        public async Task PublishMessage(string exchange, string message, string routeKey = "")
        {
            IModel? channel = null;
            try
            {
                
                channel = await GetChannel();
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange, routeKey, null, body);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                ReturnChannel(channel);
            }
        }

        public void AddConsumer(string queue, Action<IModel, BasicDeliverEventArgs> func, bool autoAck = true)
        {
            try
            {
                IModel channel = _connection.CreateModel();
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) => func(channel, ea);
                //channel.BasicQos(0, 10, false);
                channel.BasicConsume(queue, autoAck, consumer);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public void AddConsumerAsync(string queue, Func<IModel, BasicDeliverEventArgs, Task> func, bool autoAck = true)
        {
            try
            {
                IModel channel = _connection.CreateModel();
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) => await func(channel, ea);
                //channel.BasicQos(0, 30, false);
                channel.BasicConsume(queue, autoAck, consumer);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public void Dispose()
        {
            foreach (var channel in _channels)
            {
                try
                {
                    if (channel.IsOpen)
                        channel.Close();
                }
                catch (Exception e)
                {
                    _errorHandler.HandleError(e);
                }
            }
            _connection!.Dispose();
            _producerSemaphore.Dispose();
        }
    }
}
