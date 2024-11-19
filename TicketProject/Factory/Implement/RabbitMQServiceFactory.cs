using RabbitMQ.Client;
using System.Collections.Concurrent;
using TicketProject.Factory.Interfaces;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Factory.Implement
{
    public class RabbitMQServiceFactory : IRabbitMQServiceFactory
    {
        private readonly IConfiguration Configuration;
        private readonly IRetryService _retryService;
        private IConnection? _connection;
        private readonly ConcurrentBag<IModel> _channels = [];
        // 增加更多通道數量也沒有提升整體性能
        private readonly int _maxSize = 200;
        private readonly IErrorHandler<RabbitMQServiceFactory> _errorHandler;
        private readonly IErrorHandler<RabbitMQService> _serviceErrorHandler;

        public RabbitMQServiceFactory(IConfiguration configuration, IErrorHandler<RabbitMQServiceFactory> errorHandler,
            IErrorHandler<RabbitMQService> serviceErrorHandler, IRetryService retryService)
        {
            Configuration = configuration;
            _errorHandler = errorHandler;
            _serviceErrorHandler = serviceErrorHandler;
            _retryService = retryService;
        }

        private void Initialize()
        {
            try
            {
                if(_connection != null) return;

                var factory = new ConnectionFactory
                {
                    HostName = Configuration["RabbitMQ:HostName"],
                    Port = int.Parse(Configuration["RabbitMQ:Port"]!),
                    UserName = Configuration["RabbitMQ:UserName"],
                    Password = Configuration["RabbitMQ:Password"],
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedHeartbeat = TimeSpan.FromMinutes(1)
                };
                _connection = factory.CreateConnection();

                using IModel channel = _connection.CreateModel();

                channel.ExchangeDeclare("ticket_exchange", ExchangeType.Direct);
                channel.QueueDeclare(queue: "ticket_queue", true, false, false);
                channel.QueueBind("ticket_queue", "ticket_exchange", "ticket_routekey");

                for (int i = 0; i < _maxSize; i++)
                {
                    try
                    {
                        _channels.Add(_connection.CreateModel());
                        //_errorHandler.Debug($"Channel {i} created");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IRabbitMQService Create()
        {
            try
            {
                _retryService.Retry(() => Initialize(), 3000);
                return new RabbitMQService(_connection!, _serviceErrorHandler, _channels, _maxSize);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
