using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Moq;
using TicketProject.Models.Entity;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// JWTServiceTests ���O�A�Ω���� JWTService ���\��C
    /// </summary>
    public class JWTServiceTests
    {
        private readonly IJWTService _jwtService;
        private readonly Mock<IErrorHandler<JWTService>> _errorHandlerMock;

        /// <summary>
        /// �غc�禡�A��l�� JWTServiceTests ���O����ҡC
        /// </summary>
        public JWTServiceTests()
        {
            var configurationMock = new Mock<IConfiguration>();
            var keyBytes = new byte[256];
            RandomNumberGenerator.Fill(keyBytes);
            configurationMock.Setup(c => c["Jwt:SecretKey"]).Returns(Convert.ToBase64String(keyBytes));
            _errorHandlerMock = new Mock<IErrorHandler<JWTService>>();

            _jwtService = new JWTService(configurationMock.Object, _errorHandlerMock.Object);
        }

        /// <summary>
        /// ���� GenerateJwtToken ��k�A���Ҩ�O�_��������Ī��ϥΪ̥ͦ� JWT �O�P�C
        /// </summary>
        [Fact]
        public void GenerateJwtToken_ValidUser_ReturnsTokens()
        {
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@example.com",
                PhoneNumber = "0900000000",
                PasswordHash = "Passw0rd",
                IsAdmin = 1
            };

            var tokens = _jwtService.GenerateJwtToken(user);

            // �_��
            Assert.NotNull(tokens);
            Assert.Equal(2, tokens.Length);
            Assert.All(tokens, token => Assert.False(string.IsNullOrEmpty(token)));
        }

        /// <summary>
        /// ���� RefreshJwtToken ��k�A���Ҩ�O�_��������Ī��O�P�ͦ��s�� JWT �O�P�C
        /// </summary>
        [Fact]
        public void RefreshJwtToken_ValidToken_ReturnsNewTokens()
        {
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@example.com",
                PhoneNumber = "0900000000",
                PasswordHash = "Passw0rd",
                IsAdmin = 1
            };
            var tokens = _jwtService.GenerateJwtToken(user);
            var longToken = tokens[1];

            var newTokens = _jwtService.RefreshJwtToken(longToken);

            Assert.NotNull(newTokens);
            Assert.Equal(2, newTokens.Length);
            Assert.All(newTokens, token => Assert.False(string.IsNullOrEmpty(token)));
        }

        /// <summary>
        /// ���� RefreshJwtToken ��k�A���Ҩ�O�_����B�z�L�Ī��O�P�ê�^ null�C
        /// </summary>
        [Fact]
        public void RefreshJwtToken_InvalidToken_ReturnsNull()
        {
            var invalidToken = "invalid.token.value";

            var result = _jwtService.RefreshJwtToken(invalidToken);

            Assert.Null(result);
        }
    }
}
