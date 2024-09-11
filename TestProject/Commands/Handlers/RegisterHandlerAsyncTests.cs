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
    /// RegisterHandlerAsyncTests ���O�A�Ω���� RegisterHandlerAsync ���\��C
    /// </summary>
    public class RegisterHandlerAsyncTests
    {
        private readonly RegisterHandlerAsync _registerHandler;
        private readonly Mock<IUserWriteDao> _userWriteDaoMock;
        private readonly Mock<IHashService> _hashServiceMock;
        private readonly Mock<IErrorHandler<RegisterHandlerAsync>> _errorHandlerMock;
        private readonly IMapper _mapper;

        /// <summary>
        /// �غc���A��l�� RegisterHandlerAsyncTests ���O���s�������C
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
        /// ���� Handle ��k�A�ϥΦ��Ī��ШD�ê�^�ϥΪ̡C
        /// </summary>
        /// <returns>��ܫD�P�B�ާ@���u�@�C</returns>
        [Fact]
        public async Task Handle_ValidRequest_ReturnsUser()
        {
            // �w��
            var registerCommand = new RegisterCommand
            {
                // �]�m registerCommand ���ݩ�
                PasswordHash = "password"
            };

            var user = new User { UserName = "test", PasswordHash = "hashedPassword" };
            _userWriteDaoMock.Setup(u => u.AddUserAsync(It.IsAny<User>())).ReturnsAsync(user);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // ����
            var result = await _registerHandler.Handle(registerCommand, CancellationToken.None);

            // �_��
            Assert.NotNull(result);
            Assert.IsType<User>(result);
            _userWriteDaoMock.Verify(u => u.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        /// <summary>
        /// ���� Handle ��k�A��ߥX���`�ɡC
        /// </summary>
        /// <returns>��ܫD�P�B�ާ@���u�@�C</returns>
        [Fact]
        public async Task Handle_ExceptionThrown_ThrowsException()
        {
            // �w��
            var registerCommand = new RegisterCommand
            {
                // �]�m registerCommand ���ݩ�
                PasswordHash = "password"
            };

            var exception = new Exception("Test exception");
            _userWriteDaoMock.Setup(u => u.AddUserAsync(It.IsAny<User>())).ThrowsAsync(exception);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // ���� & �_��
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _registerHandler.Handle(registerCommand, CancellationToken.None);
            });

            _userWriteDaoMock.Verify(u => u.AddUserAsync(It.IsAny<User>()), Times.Once);
            _errorHandlerMock.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}
