using System.Linq.Expressions;
using Moq;
using TicketProject.Models.Entity;
using TicketProject.Querys.Handlers;
using TicketProject.DAL.Interfaces;
using TicketProject.Services.Interfaces;
using TicketProject.Querys;
using TicketProject.Services.Implement;

namespace TicketProject.Tests.Querys.Handlers
{
    /// <summary>
    /// LoginHandlerAsyncTests 類別，用於測試 LoginHandlerAsync 的功能。
    /// </summary>
    public class LoginHandlerAsyncTests
    {
        private readonly LoginHandlerAsync _loginHandler;
        private readonly IHashService _hashService;
        private readonly Mock<IUserReadDao> _userReadDaoMock;
        private readonly Mock<IErrorHandler<LoginHandlerAsync>> _errorHandlerMock;

        /// <summary>
        /// 建構式，初始化 LoginHandlerAsyncTests 類別的新執行個體。
        /// </summary>
        public LoginHandlerAsyncTests()
        {
            _userReadDaoMock = new Mock<IUserReadDao>();
            _errorHandlerMock = new Mock<IErrorHandler<LoginHandlerAsync>>();
            _hashService = new HashService();

            _loginHandler = new LoginHandlerAsync(
                _errorHandlerMock.Object,
                _userReadDaoMock.Object,
                _hashService
            );
        }

        /// <summary>
        /// 測試 Handle 方法，使用有效的請求，預期會返回 User 物件。
        /// </summary>
        [Fact]
        public async Task Handle_ValidRequest_ReturnsUser()
        {
            // 安排
            var loginQuery = new LoginQuery
            {
                Account = "test@example.com",
                Password = "password"
            };

            var user = new User { Email = "test@example.com", PasswordHash = await _hashService.HashPassword("password") };
            _userReadDaoMock.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // 執行
            var result = await _loginHandler.Handle(loginQuery, CancellationToken.None);

            // 斷言
            Assert.NotNull(result);
            Assert.IsType<User>(result);
            _userReadDaoMock.Verify(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }

        /// <summary>
        /// 測試 Handle 方法，使用無效的密碼，預期會返回 null。
        /// </summary>
        [Fact]
        public async Task Handle_InvalidPassword_ReturnsNull()
        {
            // 安排
            var loginQuery = new LoginQuery
            {
                Account = "test@example.com",
                Password = "wrongpassword"
            };

            _userReadDaoMock.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User?)null);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // 執行
            var result = await _loginHandler.Handle(loginQuery, CancellationToken.None);

            // 斷言
            Assert.Null(result);
            _userReadDaoMock.Verify(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }

        /// <summary>
        /// 測試 Handle 方法，當拋出異常時，預期會拋出異常。
        /// </summary>
        [Fact]
        public async Task Handle_ExceptionThrown_ThrowsException()
        {
            // 安排
            var loginQuery = new LoginQuery
            {
                Account = "test@example.com",
                Password = "password"
            };

            var exception = new Exception("Test exception");
            _userReadDaoMock.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>())).ThrowsAsync(exception);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // 執行 & 斷言
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _loginHandler.Handle(loginQuery, CancellationToken.None);
            });

            _userReadDaoMock.Verify(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            _errorHandlerMock.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}
