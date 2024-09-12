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
        private IDatabase? _db;

        /// <summary>
        /// ��l�� RedisService ���O���s��ҡC
        /// </summary>
        /// <param name="configuration">���ε{�����t�m�C</param>
        /// <param name="errorHandler">���~�B�z���C</param>
        public RedisService(IConfiguration configuration, IErrorHandler<RedisService> errorHandler)
        {
            _configuration = configuration;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// ��l�� Redis �s���������Ʈw��ҡC
        /// </summary>
        /// <exception cref="Exception">��s�� Redis ���ѮɩߥX�C</exception>
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
        /// �q Redis �֨��������ơC
        /// </summary>
        /// <typeparam name="T">��ƪ������C</typeparam>
        /// <param name="cacheKey">�֨���C</param>
        /// <returns>�֨�������ơA�p�G���s�b�h��^�q�{�ȡC</returns>
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
        /// �N��Ƴ]�m�� Redis �֨����C
        /// </summary>
        /// <typeparam name="T">��ƪ������C</typeparam>
        /// <param name="cacheKey">�֨���C</param>
        /// <param name="data">�n�֨�����ơC</param>
        /// <param name="expiration">�֨��L���ɶ��A�i��C</param>
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
