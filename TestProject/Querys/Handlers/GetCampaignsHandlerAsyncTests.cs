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
    /// GetCampaignsHandlerAsync ���O���椸���աC
    /// </summary>
    public class GetCampaignsHandlerAsyncTests
    {
        private readonly Mock<ICampaignReadDao> _mockCampaignReadDao;
        private readonly IDynamicQueryBuilderService<Campaign> _dynamicQueryBuilderService;
        private readonly Mock<IErrorHandler<GetCampaignsHandlerAsync>> _mockErrorHandler;
        private readonly GetCampaignsHandlerAsync _handler;

        /// <summary>
        /// ��l�� GetCampaignsHandlerAsyncTests ���O���s�������C
        /// </summary>
        public GetCampaignsHandlerAsyncTests()
        {
            _dynamicQueryBuilderService = new DynamicQueryBuilderService<Campaign>();

            _mockCampaignReadDao = new Mock<ICampaignReadDao>();
            _mockErrorHandler = new Mock<IErrorHandler<GetCampaignsHandlerAsync>>();
            _handler = new GetCampaignsHandlerAsync(_mockCampaignReadDao.Object, _mockErrorHandler.Object, _dynamicQueryBuilderService);
        }

        /// <summary>
        /// ���� GetCampaignsHandlerAsync �� Handle ��k��s�b���ʮɡC
        /// </summary>
        public async Task Handle_ShouldReturnCampaigns_WhenCampaignsExist()
        {
            // �w��
            var campaigns = new List<Campaign>
                {
                    new Campaign { CampaignName = "Test Campaign 1" },
                    new Campaign { CampaignName = "Test Campaign 2" }
                };
            var query = new GetCampaignsQuery { CampaignName = "Test Campaign 1" };
            _mockCampaignReadDao.Setup(dao => dao.GetCampaignsAsync(It.IsAny<Expression<Func<Campaign, bool>>>()))
                .ReturnsAsync(campaigns);

            // ����
            var result = await _handler.Handle(query, CancellationToken.None);

            // �_��
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// ���� GetCampaignsHandlerAsync �� Handle ��k���s�b���ʮɡC
        /// </summary>
        public async Task Handle_ShouldReturnEmptyList_WhenNoCampaignsExist()
        {
            // �w��
            var query = new GetCampaignsQuery { CampaignName = "Nonexistent Campaign" };
            _mockCampaignReadDao.Setup(dao => dao.GetCampaignsAsync(It.IsAny<Expression<Func<Campaign, bool>>>()))
                .ReturnsAsync(new List<Campaign>());

            // ����
            var result = await _handler.Handle(query, CancellationToken.None);

            // �_��
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
