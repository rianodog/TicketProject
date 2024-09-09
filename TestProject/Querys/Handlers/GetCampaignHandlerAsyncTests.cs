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
    /// GetCampaignHandlerAsync ���O���椸���աC
    /// </summary>
    public class GetCampaignHandlerAsyncTests
    {
        private readonly Mock<ICampaignReadDao> _mockCampaignReadDao;
        private readonly IDynamicQueryBuilderService<Campaign> _dynamicQueryBuilderService;
        private readonly Mock<IErrorHandler<GetCampaignHandlerAsync>> _mockErrorHandler;
        private readonly GetCampaignHandlerAsync _handler;

        /// <summary>
        /// ��l�� GetCampaignHandlerAsyncTests ���O���s�������C
        /// </summary>
        public GetCampaignHandlerAsyncTests()
        {
            _dynamicQueryBuilderService = new DynamicQueryBuilderService<Campaign>();

            _mockCampaignReadDao = new Mock<ICampaignReadDao>();
            _mockErrorHandler = new Mock<IErrorHandler<GetCampaignHandlerAsync>>();
            _handler = new GetCampaignHandlerAsync(_mockCampaignReadDao.Object, _mockErrorHandler.Object, _dynamicQueryBuilderService);
        }

        /// <summary>
        /// ���ծרҥH���ҷ�s�b�Ӭ��ʮɡAHandle ��k�O�_�|��^���ʡC
        /// </summary>
        /// <returns>��ܫD�P�B�@�~���u�@�C</returns>
        [Fact]
        public async Task Handle_ShouldReturnCampaign_WhenCampaignExists()
        {
            // �w��
            var campaign = new Campaign { CampaignName = "Test Campaign" };
            var query = new GetCampaignQuery { CampaignName = "Test Campaign" };
            _mockCampaignReadDao.Setup(dao => dao.GetCampaignAsync(It.IsAny<Expression<Func<Campaign, bool>>>()))
                .ReturnsAsync(campaign);

            // ����
            var result = await _handler.Handle(query, CancellationToken.None);

            // �_��
            Assert.NotNull(result);
            Assert.Equal("Test Campaign", result.CampaignName);
        }

        /// <summary>
        /// ���ծרҥH���ҷ�Ӭ��ʤ��s�b�ɡAHandle ��k�O�_�|��^ null�C
        /// </summary>
        /// <returns>��ܫD�P�B�@�~���u�@�C</returns>
        [Fact]
        public async Task Handle_ShouldReturnNull_WhenCampaignDoesNotExist()
        {
            // �w��
            var query = new GetCampaignQuery { CampaignName = "Nonexistent Campaign" };
            _mockCampaignReadDao.Setup(dao => dao.GetCampaignAsync(It.IsAny<Expression<Func<Campaign, bool>>>()))
                .ReturnsAsync((Campaign?)null);

            // ����
            var result = await _handler.Handle(query, CancellationToken.None);

            // �_��
            Assert.Null(result);
        }
    }
}
