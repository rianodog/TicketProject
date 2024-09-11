using AutoMapper;
using Moq;
using TicketProject.AutoMapper;
using TicketProject.Commands;
using TicketProject.Commands.Handlers;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Commands.Handlers
{
    /// <summary>
    /// RegisterHandlerAsyncTests 類別，用於測試 RegisterHandlerAsync 的功能。
    /// </summary>
    public class RegisterHandlerAsyncTests
    {
        private readonly RegisterHandlerAsync _registerHandler;
        private readonly Mock<IUserWriteDao> _userWriteDaoMock;
        private readonly Mock<IHashService> _hashServiceMock;
        private readonly Mock<IErrorHandler<RegisterHandlerAsync>> _errorHandlerMock;
        private readonly IMapper _mapper;

        /// <summary>
        /// 建構式，初始化 RegisterHandlerAsyncTests 類別的新執行個體。
        /// </summary>
        public RegisterHandlerAsyncTests()
        {
            _userWriteDaoMock = new Mock<IUserWriteDao>();
            _errorHandlerMock = new Mock<IErrorHandler<RegisterHandlerAsync>>();
            _hashServiceMock = new Mock<IHashService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommandMappingProfile>();
            });
            _mapper = config.CreateMapper();

            _registerHandler = new RegisterHandlerAsync(
                _userWriteDaoMock.Object,
                _errorHandlerMock.Object,
                _mapper,
                _hashServiceMock.Object
            );
        }

        /// <summary>
        /// 測試 Handle 方法，使用有效的請求並返回使用者。
        /// </summary>
        /// <returns>表示非同步操作的工作。</returns>
        [Fact]
        public async Task Handle_ValidRequest_ReturnsUser()
        {
            // 安排
            var registerCommand = new RegisterCommand
            {
                // 設置 registerCommand 的屬性
                PasswordHash = "password"
            };

            var user = new User { UserName = "test", PasswordHash = "hashedPassword" };
            _userWriteDaoMock.Setup(u => u.AddUserAsync(It.IsAny<User>())).ReturnsAsync(user);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // 執行
            var result = await _registerHandler.Handle(registerCommand, CancellationToken.None);

            // 斷言
            Assert.NotNull(result);
            Assert.IsType<User>(result);
            _userWriteDaoMock.Verify(u => u.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        /// <summary>
        /// 測試 Handle 方法，當拋出異常時。
        /// </summary>
        /// <returns>表示非同步操作的工作。</returns>
        [Fact]
        public async Task Handle_ExceptionThrown_ThrowsException()
        {
            // 安排
            var registerCommand = new RegisterCommand
            {
                // 設置 registerCommand 的屬性
                PasswordHash = "password"
            };

            var exception = new Exception("Test exception");
            _userWriteDaoMock.Setup(u => u.AddUserAsync(It.IsAny<User>())).ThrowsAsync(exception);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // 執行 & 斷言
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _registerHandler.Handle(registerCommand, CancellationToken.None);
            });

            _userWriteDaoMock.Verify(u => u.AddUserAsync(It.IsAny<User>()), Times.Once);
            _errorHandlerMock.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}
