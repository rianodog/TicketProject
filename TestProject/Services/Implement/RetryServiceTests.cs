using System;
using System.Threading.Tasks;
using TicketProject.Services.Implement;
using Xunit;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// RetryService 的單元測試類別。
    /// </summary>
    public class RetryServiceTests
    {
        private readonly RetryService _retryService;

        /// <summary>
        /// 初始化 RetryServiceTests 類別的新實例。
        /// </summary>
        public RetryServiceTests()
        {
            _retryService = new RetryService();
        }

        /// <summary>
        /// 測試 Retry 方法在成功時是否能正確執行。
        /// </summary>
        [Fact]
        public void Retry_ShouldExecuteSuccessfully()
        {
            // Arrange
            int executionCount = 0;
            Action action = () => executionCount++;

            // Act
            _retryService.Retry(action, 100);

            // Assert
            Assert.Equal(1, executionCount);
        }

        /// <summary>
        /// 測試 Retry 方法在達到重試次數上限時是否會拋出異常。
        /// </summary>
        [Fact]
        public void Retry_ShouldThrowException_AfterMaxRetries()
        {
            // Arrange
            int executionCount = 0;
            Action action = () =>
            {
                executionCount++;
                throw new Exception("Test exception");
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _retryService.Retry(action, 100, 3));
            Assert.Equal(3, executionCount);
        }

        /// <summary>
        /// 測試 RetryAsync 方法在成功時是否能正確執行。
        /// </summary>
        [Fact]
        public async Task RetryAsync_ShouldExecuteSuccessfully()
        {
            // Arrange
            int executionCount = 0;
            Func<Task> func = () =>
            {
                executionCount++;
                return Task.CompletedTask;
            };

            // Act
            await _retryService.RetryAsync(func, 100);

            // Assert
            Assert.Equal(1, executionCount);
        }

        /// <summary>
        /// 測試 RetryAsync 方法在達到重試次數上限時是否會拋出異常。
        /// </summary>
        [Fact]
        public async Task RetryAsync_ShouldThrowException_AfterMaxRetries()
        {
            // Arrange
            int executionCount = 0;
            Func<Task> func = () =>
            {
                executionCount++;
                throw new Exception("Test exception");
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _retryService.RetryAsync(func, 100, 3));
            Assert.Equal(3, executionCount);
        }

        /// <summary>
        /// 測試 Retry 方法在成功時是否能正確返回結果。
        /// </summary>
        [Fact]
        public void Retry_ShouldReturnResultSuccessfully()
        {
            // Arrange
            int executionCount = 0;
            Func<int> func = () =>
            {
                executionCount++;
                return 42;
            };

            // Act
            var result = _retryService.Retry(func, 100);

            // Assert
            Assert.Equal(1, executionCount);
            Assert.Equal(42, result);
        }

        /// <summary>
        /// 測試 RetryAsync 方法在成功時是否能正確返回結果。
        /// </summary>
        [Fact]
        public async Task RetryAsync_ShouldReturnResultSuccessfully()
        {
            // Arrange
            int executionCount = 0;
            Func<Task<int>> func = () =>
            {
                executionCount++;
                return Task.FromResult(42);
            };

            // Act
            var result = await _retryService.RetryAsync(func, 100);

            // Assert
            Assert.Equal(1, executionCount);
            Assert.Equal(42, result);
        }
    }
}

