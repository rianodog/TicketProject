using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    public class RetryService : IRetryService
    {
        public void Retry(Action func, int retryTimeSpan, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryTimeSpan);

            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    func();
                    break;
                }
                catch (Exception)
                {
                    if (attempts == retryCount)
                        throw;

                    Task.Delay(retryTimeSpan);
                }
            }
        }
        public async Task RetryAsync(Func<Task> func, int retryTimeSpan, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryTimeSpan);

            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    await func();
                    break;
                }
                catch (Exception)
                {
                    if (attempts == retryCount)
                        throw;

                    await Task.Delay(retryTimeSpan);
                }
            }
        }
        public T Retry<T>(Func<T> func, int retryTimeSpan, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryTimeSpan);

            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    return func();
                }
                catch (Exception)
                {
                    if (attempts == retryCount)
                        throw;

                    Task.Delay(retryTimeSpan);
                }
            }
        }

        public async Task<T> RetryAsync<T>(Func<Task<T>> func, int retryTimeSpan, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryTimeSpan);

            int attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    return await func();
                }
                catch (Exception)
                {
                    if (attempts == retryCount)
                        throw;

                    await Task.Delay(retryTimeSpan);
                }
            }
        }
    }
}
