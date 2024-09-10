namespace TicketProject.Services.Interfaces
{
    public interface IRetryService
    {
        void Retry(Action func, int retryTimeSpan, int retryCount = 3);
        T Retry<T>(Func<T> func, int retryTimeSpan, int retryCount = 3);
        Task RetryAsync(Func<Task> func, int retryTimeSpan, int retryCount = 3);
        Task<T> RetryAsync<T>(Func<Task<T>> func, int retryTimeSpan, int retryCount = 3);
    }
}
