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
        /// 測試 BcryptHashPassword 方法是否能正確返回雜湊後的密碼。
        /// </summary>
        [Fact]
        public async Task BcryptHashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            var password = "TestPassword";

            // Act
            var result = await _hashService.BcryptHashPassword(password);

            // Assert
            Assert.True(BCrypt.Net.BCrypt.Verify(password, result));
        }

        /// <summary>
        /// 測試 BcryptHashPassword 方法在發生例外時是否能正確處理錯誤。
        /// </summary>
        [Fact]
        public async Task BcryptHashPassword_ShouldHandleException()
        {
            // Arrange
            _mockErrorHandler.Setup(e => e.HandleError(It.IsAny<Exception>())).Verifiable();

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(() => _hashService.BcryptHashPassword(null!));

            // Assert
            _mockErrorHandler.Verify(e => e.HandleError(It.IsAny<Exception>()), Times.Once);
        }
    }
}
