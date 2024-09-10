namespace TicketProject.Services.Interfaces
{
    public interface IRedisService
    {
        void Initialize();
        Task<T?> GetCacheAsync<T>(string cacheKey);
        Task SetCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null);
    }
}
