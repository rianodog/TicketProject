using AutoMapper;
using Moq;
using TicketProject.Commands;
using TicketProject.Commands.Handlers;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Commands.Handlers
{

    /// <summary>  
    /// CreateEventHandlerAsyncTests ���O�A�Ω���� CreateEventHandlerAsync ���\��C  
    /// </summary>  
    public class CreateEventHandlerAsyncTests
    {
        private readonly CreateEventHandlerAsync _createEventHandler;
        private readonly Mock<IEventWriteDao> _eventWriteDaoMock;
        private readonly Mock<IErrorHandler<CreateEventHandlerAsync>> _errorHandlerMock;
        private readonly IMapper _mapper;

        /// <summary>  
        /// �غc���A��l�� CreateEventHandlerAsyncTests ���O���s�������C  
        /// </summary>  
        public CreateEventHandlerAsyncTests()
        {
            _eventWriteDaoMock = new Mock<IEventWriteDao>();
            _errorHandlerMock = new Mock<IErrorHandler<CreateEventHandlerAsync>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommandMappingProfile>();
            });
            _mapper = config.CreateMapper();

            _createEventHandler = new CreateEventHandlerAsync(
                _eventWriteDaoMock.Object,
                _errorHandlerMock.Object,
                _mapper
            );
        }

        /// <summary>  
        /// ���� <see cref="CreateEventHandlerAsync.Handle"/> ��k�ϥΦ��Ī��ШD�C  
        /// </summary>  
        /// <returns>��ܫD�P�B�@�~���u�@�C</returns>  
        [Fact]
        public async Task Handle_ValidRequest_ReturnsEventDto()
        {
            // �w��  
            var createEventCommand = new CreateEventCommand
            {
                Tickets =
                [
                    new () { TicketType = "A", Price = 10, QuantityAvailable = 10 },
                       new () { TicketType = "B", Price = 10, QuantityAvailable = 10 },
                       new () { TicketType = "C", Price = 10, QuantityAvailable = 10 }
                ]
            };

            var eventEntity = new Event { EventName = "Test Event" };
            var eventDto = new CreateEventCommand { EventName = "Test Event" };

            _eventWriteDaoMock.Setup(e => e.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(eventEntity);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // ����  
            var result = await _createEventHandler.Handle(createEventCommand, CancellationToken.None);

            // �_��  
            Assert.NotNull(result);
            Assert.IsType<CreateEventCommand>(result);
            Assert.Equal(eventDto.EventName, result.EventName);
            _eventWriteDaoMock.Verify(e => e.CreateEventAsync(It.IsAny<Event>()), Times.Once);
        }

        /// <summary>  
        /// ���� <see cref="CreateEventHandlerAsync.Handle"/> ��k�ߥX���`�����p�C  
        /// </summary>  
        /// <returns>��ܫD�P�B�@�~���u�@�C</returns>  
        [Fact]
        public async Task Handle_ExceptionThrown_ThrowsException()
        {
            // �w��  
            var createEventCommand = new CreateEventCommand
            {
                Tickets =
                [
                    new () { TicketType = "A", Price = 10, QuantityAvailable = 10 },
                       new () { TicketType = "B", Price = 10, QuantityAvailable = 10 },
                       new () { TicketType = "C", Price = 10, QuantityAvailable = 10 }
                ]
            };

            var exception = new Exception("Test exception");
            _eventWriteDaoMock.Setup(e => e.CreateEventAsync(It.IsAny<Event>())).ThrowsAsync(exception);
            _errorHandlerMock.Setup(e => e.HandleError(It.IsAny<Exception>()));

            // ���� & �_��  
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _createEventHandler.Handle(createEventCommand, CancellationToken.None);
            });

            _eventWriteDaoMock.Verify(e => e.CreateEventAsync(It.IsAny<Event>()), Times.Once);
            _errorHandlerMock.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}