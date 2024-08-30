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
    /// CreateEventHandlerAsyncTests 類別，用於測試 CreateEventHandlerAsync 的功能。  
    /// </summary>  
    public class CreateEventHandlerAsyncTests
    {
        private readonly CreateEventHandlerAsync _createEventHandler;
        private readonly Mock<IEventWriteDao> _eventWriteDaoMock;
        private readonly Mock<IErrorHandler<CreateEventHandlerAsync>> _errorHandlerMock;
        private readonly IMapper _mapper;

        /// <summary>  
        /// 建構式，初始化 CreateEventHandlerAsyncTests 類別的新執行個體。  
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
        /// 測試 <see cref="CreateEventHandlerAsync.Handle"/> 方法使用有效的請求。  
        /// </summary>  
        /// <returns>表示非同步作業的工作。</returns>  
        [Fact]
        public async Task Handle_ValidRequest_ReturnsEventDto()
        {
            // 安排  
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

            // 執行  
            var result = await _createEventHandler.Handle(createEventCommand, CancellationToken.None);

            // 斷言  
            Assert.NotNull(result);
            Assert.IsType<CreateEventCommand>(result);
            Assert.Equal(eventDto.EventName, result.EventName);
            _eventWriteDaoMock.Verify(e => e.CreateEventAsync(It.IsAny<Event>()), Times.Once);
        }

        /// <summary>  
        /// 測試 <see cref="CreateEventHandlerAsync.Handle"/> 方法拋出異常的情況。  
        /// </summary>  
        /// <returns>表示非同步作業的工作。</returns>  
        [Fact]
        public async Task Handle_ExceptionThrown_ThrowsException()
        {
            // 安排  
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

            // 執行 & 斷言  
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _createEventHandler.Handle(createEventCommand, CancellationToken.None);
            });

            _eventWriteDaoMock.Verify(e => e.CreateEventAsync(It.IsAny<Event>()), Times.Once);
            _errorHandlerMock.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}