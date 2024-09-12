using StackExchange.Redis;
using System.Text.Json;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    /// <summary>
    /// 提供 Redis 快取服務的實現。
    /// 測試於CampaignReadDaoTests進行實現。
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorHandler<RedisService> _errorHandler;
        private IDatabase? _db;

        /// <summary>
        /// 初始化 RedisService 類別的新實例。
        /// </summary>
        /// <param name="configuration">應用程式的配置。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public RedisService(IConfiguration configuration, IErrorHandler<RedisService> errorHandler)
        {
            _configuration = configuration;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 初始化 Redis 連接並獲取資料庫實例。
        /// </summary>
        /// <exception cref="Exception">當連接 Redis 失敗時拋出。</exception>
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

        /// <summary>
        /// 從 Redis 快取中獲取資料。
        /// </summary>
        /// <typeparam name="T">資料的類型。</typeparam>
        /// <param name="cacheKey">快取鍵。</param>
        /// <returns>快取中的資料，如果不存在則返回默認值。</returns>
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
                throw;
            }
        }

        /// <summary>
        /// 將資料設置到 Redis 快取中。
        /// </summary>
        /// <typeparam name="T">資料的類型。</typeparam>
        /// <param name="cacheKey">快取鍵。</param>
        /// <param name="data">要快取的資料。</param>
        /// <param name="expiration">快取過期時間，可選。</param>
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
                throw;
            }
        }

        public async Task ClearCacheAsync(string cacheKey, bool pattern = false)
        {
            try
            {
                if (pattern)
                {
                    var server = _db!.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
                    var keys = server.Keys(pattern: cacheKey + "*");

                    foreach (var key in keys)
                        await _db.KeyDeleteAsync(key);

                    return;
                }
                await _db!.KeyDeleteAsync(cacheKey);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
