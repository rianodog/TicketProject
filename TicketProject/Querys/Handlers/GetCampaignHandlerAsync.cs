using MediatR;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Querys.Handlers
{
    /// <summary>
    /// 處理 GetCampaignQuery 並根據指定的條件檢索活動。
    /// </summary>
    public class GetCampaignHandlerAsync : IRequestHandler<GetCampaignQuery, Campaign?>
    {
        private readonly ICampaignReadDao _campaignReadDao;
        private readonly IDynamicQueryBuilderService<Campaign> _dynamicQueryBuilderService;
        private readonly IErrorHandler<GetCampaignHandlerAsync> _errorHandler;

        /// <summary>
        /// 初始化 GetCampaignHandlerAsync 類別的新執行個體。
        /// </summary>
        /// <param name="campaignReadDao">活動讀取 DAO。</param>
        /// <param name="errorHandler">錯誤處理程式。</param>
        public GetCampaignHandlerAsync(ICampaignReadDao campaignReadDao, IErrorHandler<GetCampaignHandlerAsync> errorHandler, IDynamicQueryBuilderService<Campaign> dynamicQueryBuilderService)
        {
            _campaignReadDao = campaignReadDao;
            _errorHandler = errorHandler;
            _dynamicQueryBuilderService = dynamicQueryBuilderService;
        }

        /// <summary>
        /// 處理 GetCampaignQuery 並根據指定的條件檢索活動。
        /// </summary>
        /// <param name="request">GetCampaignQuery 要求。</param>
        /// <param name="cancellationToken">取消權杖。</param>
        /// <returns>檢索到的活動。</returns>
        public async Task<Campaign?> Handle(GetCampaignQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Campaign, bool>> filter = c => true;
            filter = request.CampaignName != null
                ? _dynamicQueryBuilderService.BuildFilter(filter, c => c.CampaignName == request.CampaignName) : filter;

            filter = request.Location != null
                ? _dynamicQueryBuilderService.BuildFilter(filter, c => c.Location == request.Location) : filter;

            filter = (request.CampaignStartDate != DateTime.MinValue && request.CampaignEndDate != DateTime.MinValue)
                ? _dynamicQueryBuilderService.BuildFilter(filter, c => c.CampaignDate > request.CampaignStartDate
                    && c.CampaignDate < request.CampaignEndDate) : filter;

            return await _campaignReadDao.GetCampaignAsync(filter);
        }
    }
}
