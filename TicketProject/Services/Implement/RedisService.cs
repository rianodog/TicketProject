using StackExchange.Redis;
using System.Text.Json;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    /// <summary>
    /// ���� Redis �֨��A�Ȫ���{�C
    /// ���թ�CampaignReadDaoTests�i���{�C
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorHandler<RedisService> _errorHandler;
        private readonly IDatabase _db;

        /// <summary>
        /// ��l�� RedisService ���O���s��ҡC
        /// </summary>
        /// <param name="configuration">���ε{�����t�m�C</param>
        /// <param name="errorHandler">���~�B�z���C</param>
        public RedisService(IConfiguration configuration, IErrorHandler<RedisService> errorHandler, IDatabase db)
        {
            _configuration = configuration;
            _errorHandler = errorHandler;
            _db = db;
        }

        /// <summary>
        /// �q Redis �֨��������ơC
        /// </summary>
        /// <typeparam name="T">��ƪ������C</typeparam>
        /// <param name="cacheKey">�֨���C</param>
        /// <returns>�֨�������ơA�p�G���s�b�h��^�q�{�ȡC</returns>
        public async Task<T?> GetJsonCacheAsync<T>(string cacheKey)
        {
            try
            {
                var json = (await _db.ExecuteAsync("JSON.GET", cacheKey, "."))?.ToString();

                if (string.IsNullOrEmpty(json))
                    return default;

                // �ϧǦC�� JSON �r�Ŧꬰ��H
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
        /// �N��Ƴ]�m�� Redis �֨����C
        /// </summary>
        /// <typeparam name="T">��ƪ������C</typeparam>
        /// <param name="cacheKey">�֨���C</param>
        /// <param name="data">�n�֨�����ơC</param>
        /// <param name="expiration">�֨��L���ɶ��A�i��C</param>
        public async Task SetJsonCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null)
        {
            try
            {
                string json = JsonSerializer.Serialize(data);

                // ���� RedisJSON �� JSON.SET �R�O
                await _db.ExecuteAsync("JSON.SET", cacheKey, ".", json);

                // �p�G�ݭn�]�m�L���ɶ��A�ϥ� KeyExpire
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
