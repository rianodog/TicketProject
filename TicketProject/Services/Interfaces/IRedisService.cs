
namespace TicketProject.Services.Interfaces
{
    public interface IRedisService
    {
        Task<T?> GetJsonCacheAsync<T>(string cacheKey);
        Task SetJsonCacheAsync<T>(string cacheKey, T data, TimeSpan? expiration = null);
        Task ClearCacheAsync(string cacheKey, bool pattern = false);
        Task<bool> TryGetLockAsync(string key, TimeSpan expiration);
        Task<bool> ReleaseLockAsync(string key);
        Task StartLockExtensionTaskAsync(string key, TimeSpan expirationMS, int maxExtensionAttempts, CancellationTokenSource tokenSource);
        Task<string> GetStringCacheAsync(string cacheKey);
        Task SetStringCacheAsync(string cacheKey, string data, TimeSpan? expiration = null);
        Task<string> ExecuteLuaScriptAsync(string script, string[] keys, string[] args);
    }
}
