using System;
using System.Threading.Tasks;
using TicketProject.Services.Implement;
using Xunit;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// RetryService ���椸�������O�C
    /// </summary>
    public class RetryServiceTests
    {
        private readonly RetryService _retryService;

        /// <summary>
        /// ��l�� RetryServiceTests ���O���s��ҡC
        /// </summary>
        public RetryServiceTests()
        {
            _retryService = new RetryService();
        }

        /// <summary>
        /// ���� Retry ��k�b���\�ɬO�_�ॿ�T����C
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
        /// ���� Retry ��k�b�F�쭫�զ��ƤW���ɬO�_�|�ߥX���`�C
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
        /// ���� RetryAsync ��k�b���\�ɬO�_�ॿ�T����C
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
        /// ���� RetryAsync ��k�b�F�쭫�զ��ƤW���ɬO�_�|�ߥX���`�C
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
        /// ���� Retry ��k�b���\�ɬO�_�ॿ�T��^���G�C
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
        /// ���� RetryAsync ��k�b���\�ɬO�_�ॿ�T��^���G�C
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

