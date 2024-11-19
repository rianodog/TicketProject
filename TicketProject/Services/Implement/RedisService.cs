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
        private readonly IDatabase _db;

        /// <summary>
        /// 初始化 RedisService 類別的新實例。
        /// </summary>
        /// <param name="configuration">應用程式的配置。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public RedisService(IConfiguration configuration, IErrorHandler<RedisService> errorHandler, IDatabase db)
        {
            _configuration = configuration;
            _errorHandler = errorHandler;
            _db = db;
        }

        /// <summary>
        /// 從 Redis 快取中獲取資料。
        /// </summary>
        /// <typeparam name="T">資料的類型。</typeparam>
        /// <param name="cacheKey">快取鍵。</param>
        /// <returns>快取中的資料，如果不存在則返回默認值。</returns>
        public async Task<T?> GetJsonCacheAsync<T>(string cacheKey)
        {
            try
            {
                var json = (await _db.ExecuteAsync("JSON.GET", cacheKey, "."))?.ToString();

                if (string.IsNullOrEmpty(json))
                    return default;

                // 反序列化 JSON 字符串為對象
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task<string> GetStringCacheAsync(string cacheKey)
        {
            try
            {
                var cachedData = await _db!.StringGetAsync(cacheKey);
                if (cachedData.HasValue)
                {
                    return cachedData!;
                }
                return "";
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task SetStringCacheAsync(string cacheKey, string data, TimeSpan? expiration = null)
        {
            try
            {
                if (expiration == null)
                {
                    await _db!.StringSetAsync(cacheKey, data);
                    return;
                }
                await _db!.StringSetAsync(cacheKey, data, expiration);
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
        public async Task SetJsonCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null)
        {
            try
            {
                string json = JsonSerializer.Serialize(data);

                // 執行 RedisJSON 的 JSON.SET 命令
                await _db.ExecuteAsync("JSON.SET", cacheKey, ".", json);

                // 如果需要設置過期時間，使用 KeyExpire
                if (expiration.HasValue)
                    await _db.KeyExpireAsync(cacheKey, expiration);
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

        public async Task<bool> TryGetLockAsync(string key, TimeSpan expiration)
        {
            try
            {
                return await _db!.StringSetAsync(key, "lock", expiration, When.NotExists);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task<string> ExecuteLuaScriptAsync(string script, string[] keys, string[] args)
        {
            var prepared = LuaScript.Prepare(script);
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            var redisValues = args.Select(a => (RedisValue)a).ToArray();
            var result = await _db.ScriptEvaluateAsync(script, redisKeys, redisValues);
            if (result.IsNull)
                return string.Empty;
            return result.ToString();
        }

        public async Task<bool> ReleaseLockAsync(string key)
        {
            try
            {
                return await _db!.KeyDeleteAsync(key);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task StartLockExtensionTaskAsync(string key, TimeSpan expirationMS, int maxExtensionAttempts, CancellationTokenSource tokenSource)
        {
            try
            {
                var extensionAttempts = 0;
                var extendLockScript = @"
                local key = KEYS[1]
                local expiration = tonumber(ARGV[1])

                if redis.call('get', key) == 'lock' then
                    return redis.call('pexpire', key, expiration)
                else
                    return 0
                end
                ";

                await Task.Run(async () =>
                {

                    while (!tokenSource.Token.IsCancellationRequested && extensionAttempts < maxExtensionAttempts)
                    {
                        await Task.Delay(expirationMS / 2, tokenSource.Token);
                        
                        var lockExtended = await ExecuteLuaScriptAsync(extendLockScript, [key], [((int)expirationMS.TotalMilliseconds).ToString()]);

                        if (string.IsNullOrEmpty(lockExtended) || lockExtended == "0")
                            tokenSource.Cancel();

                        if (++extensionAttempts >= maxExtensionAttempts)
                            tokenSource.Cancel();
                    }
                }, tokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                tokenSource.Dispose();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
