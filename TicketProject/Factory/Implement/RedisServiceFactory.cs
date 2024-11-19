using StackExchange.Redis;
using TicketProject.Factory.Interfaces;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Factory.Implement
{
    public class RedisServiceFactory : IRedisServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IRetryService _retryService;
        private ConnectionMultiplexer? _connectionMultiplexer;
        private readonly IErrorHandler<RedisServiceFactory> _errorHandler;
        private readonly IErrorHandler<RedisService> _redisErrorHandler;

        public RedisServiceFactory(IConfiguration configuration, IRetryService retryService, IErrorHandler<RedisServiceFactory> errorHandler, IErrorHandler<RedisService> redisErrorHandler)
        {
            _configuration = configuration;
            _retryService = retryService;
            _errorHandler = errorHandler;
            _redisErrorHandler = redisErrorHandler;
        }

        private void Initialize()
        {
            try
            {
                if(_connectionMultiplexer != null) return;
                var options = ConfigurationOptions.Parse(_configuration.GetConnectionString("Redis")!);
                options.ClientName = "TicketProject";
                options.ConnectRetry = 3;
                options.ConnectTimeout = 10000;
                options.SyncTimeout = 10000;
                _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IRedisService Create()
        {
            try
            {
                _retryService.Retry(() => Initialize(), 3000);
                return new RedisService(_configuration, _redisErrorHandler, _connectionMultiplexer!.GetDatabase());
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
