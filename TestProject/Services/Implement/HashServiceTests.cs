using Moq;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// HashService ���椸�������O�C
    /// </summary>
    public class HashServiceTests
    {
        private readonly Mock<IErrorHandler<HashService>> _mockErrorHandler;
        private readonly HashService _hashService;

        /// <summary>
        /// ��l�� HashServiceTests ���O���s�������C
        /// </summary>
        public HashServiceTests()
        {
            _mockErrorHandler = new Mock<IErrorHandler<HashService>>();
            _hashService = new HashService(_mockErrorHandler.Object);
        }

        /// <summary>
        /// ���� BcryptHashPassword ��k�O�_�ॿ�T��^����᪺�K�X�C
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
        /// ���� BcryptHashPassword ��k�b�o�ͨҥ~�ɬO�_�ॿ�T�B�z���~�C
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
