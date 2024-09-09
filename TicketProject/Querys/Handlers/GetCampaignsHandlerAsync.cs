using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Querys.Handlers
{
    /// <summary>
    /// 處理 GetCampaignsQuery 並檢索一個活動集合。
    /// </summary>
    public class GetCampaignsHandlerAsync : IRequestHandler<GetCampaignsQuery, ICollection<Campaign>>
    {
        private readonly ICampaignReadDao _campaignReadDao;
        private readonly IDynamicQueryBuilderService<Campaign> _dynamicQueryBuilderService;
        private readonly IErrorHandler<GetCampaignsHandlerAsync> _errorHandler;

        /// <summary>
        /// 初始化 GetCampaignsHandlerAsync 類別的新執行個體。
        /// </summary>
        /// <param name="campaignReadDao">活動讀取 DAO。</param>
        /// <param name="errorHandler">錯誤處理程式。</param>
        public GetCampaignsHandlerAsync(ICampaignReadDao campaignReadDao, IErrorHandler<GetCampaignsHandlerAsync> errorHandler, IDynamicQueryBuilderService<Campaign> dynamicQueryBuilderService)
        {
            _campaignReadDao = campaignReadDao;
            _errorHandler = errorHandler;
            _dynamicQueryBuilderService = dynamicQueryBuilderService;
        }

        /// <summary>
        /// 處理 GetCampaignsQuery 並根據指定的篩選條件檢索一個活動集合。
        /// </summary>
        /// <param name="request">GetCampaignsQuery 要求。</param>
        /// <param name="cancellationToken">取消權杖。</param>
        /// <returns>一個活動集合。</returns>
        public async Task<ICollection<Campaign>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Campaign, bool>> filter = c => true;

            filter = request.CampaignName != null
                ? _dynamicQueryBuilderService.BuildFilter(filter, c => c.CampaignName == request.CampaignName) : filter;

            filter = request.Location != null
                ? _dynamicQueryBuilderService.BuildFilter(filter, c => c.Location == request.Location) : filter;

            filter = (request.CampaignStartDate != DateTime.MinValue && request.CampaignEndDate != DateTime.MinValue)
                ? _dynamicQueryBuilderService.BuildFilter(filter, c => c.CampaignDate > request.CampaignStartDate
                    && c.CampaignDate < request.CampaignEndDate) : filter;

            return await _campaignReadDao.GetCampaignsAsync(filter);
        }
    }
}
