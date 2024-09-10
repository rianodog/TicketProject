using TicketProject.Factory.Interfaces;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Factory.Implement
{
    public class RedisServiceFactory : IRedisServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IRetryService _retryService;
        private readonly IErrorHandler<RedisServiceFactory> _errorHandler;
        private readonly IErrorHandler<RedisService> _redisErrorHandler;

        public RedisServiceFactory(IConfiguration configuration, IRetryService retryService, IErrorHandler<RedisServiceFactory> errorHandler, IErrorHandler<RedisService> redisErrorHandler)
        {
            _configuration = configuration;
            _retryService = retryService;
            _errorHandler = errorHandler;
            _redisErrorHandler = redisErrorHandler;
        }
        public IRedisService Create()
        {
            try
            {
                var redisService = new RedisService(_configuration, _redisErrorHandler);
                _retryService.Retry(() => redisService.Initialize(), 3000);
                return redisService;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
