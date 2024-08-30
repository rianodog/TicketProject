using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Moq;
using TicketProject.Models.Entity;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Services.Implement
{
    /// <summary>
    /// JWTServiceTests 類別，用於測試 JWTService 的功能。
    /// </summary>
    public class JWTServiceTests
    {
        private readonly IJWTService _jwtService;
        private readonly Mock<IErrorHandler<JWTService>> _errorHandlerMock;

        /// <summary>
        /// 建構函式，初始化 JWTServiceTests 類別的實例。
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
        /// 測試 GenerateJwtToken 方法，驗證其是否能夠為有效的使用者生成 JWT 令牌。
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

            // 斷言
            Assert.NotNull(tokens);
            Assert.Equal(2, tokens.Length);
            Assert.All(tokens, token => Assert.False(string.IsNullOrEmpty(token)));
        }

        /// <summary>
        /// 測試 RefreshJwtToken 方法，驗證其是否能夠為有效的令牌生成新的 JWT 令牌。
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
        /// 測試 RefreshJwtToken 方法，驗證其是否能夠處理無效的令牌並返回 null。
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
