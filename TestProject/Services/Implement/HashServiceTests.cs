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
        /// ���� HashPassword ��k�O�_�ॿ�T��^����᪺�K�X�C
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
        /// ���� HashPassword ��k�b�o�ͨҥ~�ɬO�_�ॿ�T�B�z���~�C
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
