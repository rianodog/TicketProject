using MediatR;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Dto;
using TicketProject.Services.Interfaces;

namespace TicketProject.Querys.Handlers
{
    /// <summary>
    /// 處理 GetCampaignsQuery 並檢索一個活動集合。
    /// </summary>
    public class GetCampaignFormIdHandlerAsync : IRequestHandler<GetCampaignFormIdQuery, CampaignDto>
    {
        private readonly ICampaignReadDao _campaignReadDao;
        private readonly IErrorHandler<GetCampaignFormIdHandlerAsync> _errorHandler;

        /// <summary>
        /// 初始化 GetCampaignsHandlerAsync 類別的新實例。
        /// </summary>
        /// <param name="campaignReadDao">活動讀取 DAO。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        /// <param name="dynamicQueryBuilderService">動態查詢構建服務。</param>
        public GetCampaignFormIdHandlerAsync(ICampaignReadDao campaignReadDao, IErrorHandler<GetCampaignFormIdHandlerAsync> errorHandler)
        {
            _campaignReadDao = campaignReadDao;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 處理 GetCampaignsQuery 並根據指定的篩選條件檢索活動集合。
        /// </summary>
        /// <param name="request">GetCampaignsQuery 請求。</param>
        /// <param name="cancellationToken">取消權杖。</param>
        /// <returns>檢索到的活動集合。</returns>
        public async Task<CampaignDto> Handle(GetCampaignFormIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _campaignReadDao.GetCampgignFormIdAsync(request.CampaignId);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
