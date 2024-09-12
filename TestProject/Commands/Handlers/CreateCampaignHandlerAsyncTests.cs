using AutoMapper;
using Moq;
using TicketProject.AutoMapper;
using TicketProject.Commands;
using TicketProject.Commands.Handlers;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;
using static TicketProject.Models.Enums;

namespace TicketProject.Tests.Commands.Handlers
{
    /// <summary>
    /// CreateCampaignHandlerAsync 的單元測試類別。
    /// </summary>
    public class CreateCampaignHandlerAsyncTests
    {
        private readonly Mock<ICampaignWriteDao> _mockCampaignWriteDao;
        private readonly Mock<IErrorHandler<CreateCampaignHandlerAsync>> _mockErrorHandler;
        private readonly Mock<IRedisService> _mockRedisService;
        private readonly IMapper _mapper;
        private readonly CreateCampaignHandlerAsync _handler;

        /// <summary>
        /// 初始化 CreateCampaignHandlerAsyncTests 類別的新實例。
        /// </summary>
        public CreateCampaignHandlerAsyncTests()
        {
            _mockCampaignWriteDao = new Mock<ICampaignWriteDao>();
            _mockErrorHandler = new Mock<IErrorHandler<CreateCampaignHandlerAsync>>();
            _mockRedisService = new Mock<IRedisService>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommandMappingProfile>();
            });
            _mapper = config.CreateMapper();
            _handler = new CreateCampaignHandlerAsync(_mockCampaignWriteDao.Object, _mockErrorHandler.Object, _mapper, _mockRedisService.Object);
        }

        private CreateCampaignCommand CreateSampleCommand()
        {
            return new CreateCampaignCommand
            {
                CampaignName = "Test Campaign",
                Description = "Test Description",
                Location = "Test Location",
                CampaignDate = DateTime.UtcNow,
                TicketContents =
                    {
                        new CreateCampaign_TicketContentDto
                        {
                            TypeName = (TicketType)1,
                            QuantityAvailable = 100,
                            Price = 150.00m
                        },
                        new CreateCampaign_TicketContentDto
                        {
                            TypeName = (TicketType)2,
                            QuantityAvailable = 200,
                            Price = 50.00m
                        }
                    }
            };
        }

        private Campaign CreateSampleCampaign(CreateCampaignCommand command)
        {
            return new Campaign
            {
                CampaignName = command.CampaignName,
                Description = command.Description,
                Location = command.Location,
                CampaignDate = command.CampaignDate,
                TicketContents = command.TicketContents.Select(tc => new TicketContent
                {
                    TypeName = tc.TypeName.ToString(),
                    QuantityAvailable = tc.QuantityAvailable,
                    Price = tc.Price
                }).ToList()
            };
        }

        /// <summary>
        /// 測試 Handle 方法是否能成功建立活動並返回命令。
        /// </summary>
        [Fact]
        public async Task Handle_ShouldCreateCampaignAndReturnCommand()
        {
            // Arrange
            var command = CreateSampleCommand();
            var campaign = CreateSampleCampaign(command);

            _mockCampaignWriteDao.Setup(d => d.CreateCampaignAsync(It.IsAny<Campaign>())).ReturnsAsync(campaign);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(command.CampaignName, result.CampaignName);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Location, result.Location);
            Assert.Equal(command.CampaignDate, result.CampaignDate);
            Assert.Equal(command.TicketContents.Count, result.TicketContents.Count);
            for (int i = 0; i < command.TicketContents.Count; i++)
            {
                Assert.Equal(command.TicketContents.ElementAt(i).TypeName, result.TicketContents.ElementAt(i).TypeName);
                Assert.Equal(command.TicketContents.ElementAt(i).QuantityAvailable, result.TicketContents.ElementAt(i).QuantityAvailable);
                Assert.Equal(command.TicketContents.ElementAt(i).Price, result.TicketContents.ElementAt(i).Price);
            }

            _mockCampaignWriteDao.Verify(d => d.CreateCampaignAsync(It.IsAny<Campaign>()), Times.Once);
            _mockRedisService.Verify(r => r.ClearCacheAsync("Campaigns", false), Times.Once);
        }

        /// <summary>
        /// 測試 Handle 方法在發生異常時是否能正確處理。
        /// </summary>
        [Fact]
        public async Task Handle_ShouldHandleException()
        {
            // Arrange
            var command = CreateSampleCommand();
            var exception = new Exception("Test exception");
            _mockCampaignWriteDao.Setup(d => d.CreateCampaignAsync(It.IsAny<Campaign>())).ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _mockErrorHandler.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}
