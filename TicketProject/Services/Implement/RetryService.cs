using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    /// <summary>
    /// 提供重試機制的服務。
    /// </summary>
    public class RetryService : IRetryService
    {
        /// <summary>
        /// 同步重試指定的操作。
        /// </summary>
        /// <param name="func">要重試的操作。</param>
        /// <param name="ms">重試之間的時間間隔（毫秒）。</param>
        /// <param name="retryCount">重試次數，默認為3次。</param>
        /// <exception cref="ArgumentOutOfRangeException">當 retryCount 或 ms 為負數或零時拋出。</exception>
        /// <exception cref="Exception">當重試次數達到上限時拋出。</exception>
        public void Retry(Action func, int ms, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ms);

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

                    Task.Delay(ms).Wait();
                }
            }
        }

        /// <summary>
        /// 異步重試指定的操作。
        /// </summary>
        /// <param name="func">要重試的異步操作。</param>
        /// <param name="ms">重試之間的時間間隔（毫秒）。</param>
        /// <param name="retryCount">重試次數，默認為3次。</param>
        /// <returns>一個表示異步操作的 Task。</returns>
        /// <exception cref="ArgumentOutOfRangeException">當 retryCount 或 ms 為負數或零時拋出。</exception>
        /// <exception cref="Exception">當重試次數達到上限時拋出。</exception>
        public async Task RetryAsync(Func<Task> func, int ms, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ms);

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

                    await Task.Delay(ms);
                }
            }
        }

        public async Task<bool> RetryAsync(Func<Task<bool>> func, int retryTimeSpan, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryTimeSpan);

            int attempts = 0;
            while (true)
            {
                if (await func())
                    return true;

                if (++attempts == retryCount)
                    return false;

                await Task.Delay(retryTimeSpan + (attempts * retryTimeSpan));
            }
        }

        /// <summary>
        /// 同步重試指定的操作並返回結果。
        /// </summary>
        /// <typeparam name="T">操作返回的結果類型。</typeparam>
        /// <param name="func">要重試的操作。</param>
        /// <param name="ms">重試之間的時間間隔（毫秒）。</param>
        /// <param name="retryCount">重試次數，默認為3次。</param>
        /// <returns>操作的結果。</returns>
        /// <exception cref="ArgumentOutOfRangeException">當 retryCount 或 ms 為負數或零時拋出。</exception>
        /// <exception cref="Exception">當重試次數達到上限時拋出。</exception>
        public T Retry<T>(Func<T> func, int ms, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ms);

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

                    Task.Delay(ms).Wait();
                }
            }
        }

        /// <summary>
        /// 異步重試指定的操作並返回結果。
        /// </summary>
        /// <typeparam name="T">操作返回的結果類型。</typeparam>
        /// <param name="func">要重試的異步操作。</param>
        /// <param name="ms">重試之間的時間間隔（毫秒）。</param>
        /// <param name="retryCount">重試次數，默認為3次。</param>
        /// <returns>操作的結果。</returns>
        /// <exception cref="ArgumentOutOfRangeException">當 retryCount 或 ms 為負數或零時拋出。</exception>
        /// <exception cref="Exception">當重試次數達到上限時拋出。</exception>
        public async Task<T> RetryAsync<T>(Func<Task<T>> func, int ms, int retryCount = 3)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ms);

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

                    await Task.Delay(ms);
                }
            }
        }
    }
}
