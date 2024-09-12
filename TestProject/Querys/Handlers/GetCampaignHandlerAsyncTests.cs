using System.Linq.Expressions;
using Moq;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Querys;
using TicketProject.Querys.Handlers;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Querys.Handlers
{
    /// <summary>
    /// GetCampaignHandlerAsync 的單元測試類別。
    /// </summary>
    public class GetCampaignHandlerAsyncTests
    {
        private readonly Mock<ICampaignReadDao> _mockCampaignReadDao;
        private readonly Mock<IDynamicQueryBuilderService<Campaign>> _mockDynamicQueryBuilderService;
        private readonly Mock<IErrorHandler<GetCampaignHandlerAsync>> _mockErrorHandler;
        private readonly GetCampaignHandlerAsync _handler;

        /// <summary>
        /// 初始化 GetCampaignHandlerAsyncTests 類別的新實例。
        /// </summary>
        public GetCampaignHandlerAsyncTests()
        {
            _mockCampaignReadDao = new Mock<ICampaignReadDao>();
            _mockDynamicQueryBuilderService = new Mock<IDynamicQueryBuilderService<Campaign>>();
            _mockErrorHandler = new Mock<IErrorHandler<GetCampaignHandlerAsync>>();
            _handler = new GetCampaignHandlerAsync(_mockCampaignReadDao.Object, _mockErrorHandler.Object, _mockDynamicQueryBuilderService.Object);
        }

        private void SetupMockCampaignReadDao(List<Campaign> campaigns)
        {
            _mockCampaignReadDao.Setup(d => d.GetCampaignAsync(It.IsAny<Expression<Func<Campaign, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(campaigns);
        }

        private void SetupMockDynamicQueryBuilderService()
        {
            _mockDynamicQueryBuilderService.Setup(d => d.BuildFilter(It.IsAny<Expression<Func<Campaign, bool>>>(), It.IsAny<Expression<Func<Campaign, bool>>>(), It.IsAny<string>()))
                .Returns<Expression<Func<Campaign, bool>>, Expression<Func<Campaign, bool>>, string>((current, newFilter, str) => newFilter);
        }

        /// <summary>
        /// 測試 Handle 方法在沒有篩選條件時是否能正確返回所有活動。
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnAllCampaigns_WhenNoFiltersProvided()
        {
            // Arrange
            var request = new GetCampaignQuery();
            var campaigns = new List<Campaign> { new Campaign { CampaignId = 1, CampaignName = "Test Campaign" } };
            SetupMockCampaignReadDao(campaigns);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Campaign", result.First().CampaignName);
        }

        /// <summary>
        /// 測試 Handle 方法在提供篩選條件時是否能正確返回符合條件的活動。
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnFilteredCampaigns_WhenFiltersProvided()
        {
            // Arrange
            var request = new GetCampaignQuery { CampaignName = "Test Campaign" };
            var campaigns = new List<Campaign> { new Campaign { CampaignId = 1, CampaignName = "Test Campaign" } };
            SetupMockCampaignReadDao(campaigns);
            SetupMockDynamicQueryBuilderService();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Campaign", result.First().CampaignName);
        }

        /// <summary>
        /// 測試 Handle 方法在提供城市篩選條件時是否能正確使用快取。
        /// </summary>
        [Fact]
        public async Task Handle_ShouldUseCache_WhenCityFilterProvided()
        {
            // Arrange
            var request = new GetCampaignQuery { City = "SampleCity" };
            var campaigns = new List<Campaign> { new Campaign { CampaignId = 1, CampaignName = "Test Campaign" } };

            _mockCampaignReadDao.Setup(d => d.GetCampaignAsync(It.IsAny<Expression<Func<Campaign, bool>>>(), "SampleCity"))
                .ReturnsAsync(campaigns);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Campaign", result.First().CampaignName);
        }

        /// <summary>
        /// 測試 Handle 方法在發生異常時是否能正確處理錯誤。
        /// </summary>
        [Fact]
        public async Task Handle_ShouldHandleError_WhenExceptionThrown()
        {
            // Arrange
            var request = new GetCampaignQuery();
            var exception = new Exception("Test exception");

            _mockCampaignReadDao.Setup(d => d.GetCampaignAsync(It.IsAny<Expression<Func<Campaign, bool>>>(), It.IsAny<string>()))
                .ThrowsAsync(exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            _mockErrorHandler.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}

