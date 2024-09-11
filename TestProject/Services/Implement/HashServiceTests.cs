using Moq;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// HashService 的單元測試類別。
    /// </summary>
    public class HashServiceTests
    {
        private readonly Mock<IErrorHandler<HashService>> _mockErrorHandler;
        private readonly HashService _hashService;

        /// <summary>
        /// 初始化 HashServiceTests 類別的新執行個體。
        /// </summary>
        public HashServiceTests()
        {
            _mockErrorHandler = new Mock<IErrorHandler<HashService>>();
            _hashService = new HashService(_mockErrorHandler.Object);
        }

        /// <summary>
        /// 測試 HashPassword 方法是否能正確返回雜湊後的密碼。
        /// </summary>
        [Fact]
        public async Task HashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            var password = "TestPassword";
            var expectedHash = "7bcf9d89298f1bfae16fa02ed6b61908fd2fa8de45dd8e2153a3c47300765328";

            // Act
            var result = await _hashService.HashPassword(password);

            // Assert
            Assert.Equal(expectedHash, result);
        }

        /// <summary>
        /// 測試 HashPassword 方法在發生例外時是否能正確處理錯誤。
        /// </summary>
        [Fact]
        public async Task HashPassword_ShouldHandleException()
        {
            // Arrange
            _mockErrorHandler.Setup(e => e.HandleError(It.IsAny<Exception>())).Verifiable();

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(() => _hashService.HashPassword(null!));

            // Assert
            _mockErrorHandler.Verify(e => e.HandleError(It.IsAny<Exception>()), Times.Once);
        }
    }
}
