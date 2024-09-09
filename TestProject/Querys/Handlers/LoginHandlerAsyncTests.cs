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
    /// LoginHandlerAsyncTests ���O�A�Ω���� LoginHandlerAsync ���\��C
    /// </summary>
    public class LoginHandlerAsyncTests
    {
        private readonly LoginHandlerAsync _loginHandler;
        private readonly IHashService _hashService;
        private readonly Mock<IUserReadDao> _userReadDaoMock;
        private readonly Mock<IErrorHandler<LoginHandlerAsync>> _errorHandlerMock;

        /// <summary>
        /// �غc���A��l�� LoginHandlerAsyncTests ���O���s�������C
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
        /// ���� Handle ��k�A�ϥΦ��Ī��ШD�A�w���|��^ User ����C
        /// </summary>
        [Fact]
        public async Task Handle_ValidRequest_ReturnsUser()
        {
            // �w��
            var loginQuery = new LoginQuery
            {
                Account = "test@example.com",
                Password = "password"
            };

            var user = new User { Email = "test@example.com", PasswordHash = await _hashService.HashPassword("password") };
            _userReadDaoMock.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // ����
            var result = await _loginHandler.Handle(loginQuery, CancellationToken.None);

            // �_��
            Assert.NotNull(result);
            Assert.IsType<User>(result);
            _userReadDaoMock.Verify(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }

        /// <summary>
        /// ���� Handle ��k�A�ϥεL�Ī��K�X�A�w���|��^ null�C
        /// </summary>
        [Fact]
        public async Task Handle_InvalidPassword_ReturnsNull()
        {
            // �w��
            var loginQuery = new LoginQuery
            {
                Account = "test@example.com",
                Password = "wrongpassword"
            };

            _userReadDaoMock.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User?)null);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // ����
            var result = await _loginHandler.Handle(loginQuery, CancellationToken.None);

            // �_��
            Assert.Null(result);
            _userReadDaoMock.Verify(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }

        /// <summary>
        /// ���� Handle ��k�A��ߥX���`�ɡA�w���|�ߥX���`�C
        /// </summary>
        [Fact]
        public async Task Handle_ExceptionThrown_ThrowsException()
        {
            // �w��
            var loginQuery = new LoginQuery
            {
                Account = "test@example.com",
                Password = "password"
            };

            var exception = new Exception("Test exception");
            _userReadDaoMock.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>())).ThrowsAsync(exception);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // ���� & �_��
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _loginHandler.Handle(loginQuery, CancellationToken.None);
            });

            _userReadDaoMock.Verify(u => u.GetUserAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            _errorHandlerMock.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}
