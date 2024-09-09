using Moq;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Querys;
using TicketProject.Querys.Handlers;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Tests.Querys.Handlers
{
    /// <summary>
    /// GetCampaignHandlerAsync 類別的單元測試。
    /// </summary>
    public class GetCampaignHandlerAsyncTests
    {
        private readonly Mock<ICampaignReadDao> _mockCampaignReadDao;
        private readonly IDynamicQueryBuilderService<Campaign> _dynamicQueryBuilderService;
        private readonly Mock<IErrorHandler<GetCampaignHandlerAsync>> _mockErrorHandler;
        private readonly GetCampaignHandlerAsync _handler;

        /// <summary>
        /// 初始化 GetCampaignHandlerAsyncTests 類別的新執行個體。
        /// </summary>
        public GetCampaignHandlerAsyncTests()
        {
            _dynamicQueryBuilderService = new DynamicQueryBuilderService<Campaign>();

            _mockCampaignReadDao = new Mock<ICampaignReadDao>();
            _mockErrorHandler = new Mock<IErrorHandler<GetCampaignHandlerAsync>>();
            _handler = new GetCampaignHandlerAsync(_mockCampaignReadDao.Object, _mockErrorHandler.Object, _dynamicQueryBuilderService);
        }

        /// <summary>
        /// 測試案例以驗證當存在該活動時，Handle 方法是否會返回活動。
        /// </summary>
        /// <returns>表示非同步作業的工作。</returns>
        [Fact]
        public async Task Handle_ShouldReturnCampaign_WhenCampaignExists()
        {
            // 安排
            var campaign = new Campaign { CampaignName = "Test Campaign" };
            var query = new GetCampaignQuery { CampaignName = "Test Campaign" };
            _mockCampaignReadDao.Setup(dao => dao.GetCampaignAsync(It.IsAny<Expression<Func<Campaign, bool>>>()))
                .ReturnsAsync(campaign);

            // 執行
            var result = await _handler.Handle(query, CancellationToken.None);

            // 斷言
            Assert.NotNull(result);
            Assert.Equal("Test Campaign", result.CampaignName);
        }

        /// <summary>
        /// 測試案例以驗證當該活動不存在時，Handle 方法是否會返回 null。
        /// </summary>
        /// <returns>表示非同步作業的工作。</returns>
        [Fact]
        public async Task Handle_ShouldReturnNull_WhenCampaignDoesNotExist()
        {
            // 安排
            var query = new GetCampaignQuery { CampaignName = "Nonexistent Campaign" };
            _mockCampaignReadDao.Setup(dao => dao.GetCampaignAsync(It.IsAny<Expression<Func<Campaign, bool>>>()))
                .ReturnsAsync((Campaign?)null);

            // 執行
            var result = await _handler.Handle(query, CancellationToken.None);

            // 斷言
            Assert.Null(result);
        }
    }
}
