using StackExchange.Redis;
using System.Text.Json;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    public class RedisService : IRedisService
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorHandler<RedisService> _errorHandler;
        // 由初始化時指派 不指定readonly
        private IDatabase? _db;

        public RedisService(IConfiguration configuration, IErrorHandler<RedisService> errorHandler)
        {
            _configuration = configuration;
            _errorHandler = errorHandler;
        }

        public void Initialize()
        {
            try
            {
                var redisConnection = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis")!);
                _db = redisConnection.GetDatabase();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task<T?> GetCacheAsync<T>(string cacheKey)
        {
            try
            {
                var cachedData = await _db!.StringGetAsync(cacheKey);
                if (cachedData.HasValue)
                {
                    return JsonSerializer.Deserialize<T>(cachedData!);
                }
                return default;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                return default;
            }
        }

        public async Task SetCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null)
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(data!);
                if (expiration == null)
                {
                    await _db!.StringSetAsync(cacheKey, serializedData);
                    return;
                }
                await _db!.StringSetAsync(cacheKey, serializedData, expiration);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
        }
    }
}
