﻿using MediatR;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Querys.Handlers
{
    /// <summary>
    /// 處理 GetCampaignsQuery 並檢索一個活動集合。
    /// </summary>
    public class GetCampaignsHandlerAsync : IRequestHandler<GetCampaignsQuery, ICollection<CampaignDto>>
    {
        private readonly ICampaignReadDao _campaignReadDao;
        private readonly IDynamicQueryBuilderService<Campaign> _dynamicQueryBuilderService;
        private readonly IErrorHandler<GetCampaignsHandlerAsync> _errorHandler;

        /// <summary>
        /// 初始化 GetCampaignsHandlerAsync 類別的新實例。
        /// </summary>
        /// <param name="campaignReadDao">活動讀取 DAO。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        /// <param name="dynamicQueryBuilderService">動態查詢構建服務。</param>
        public GetCampaignsHandlerAsync(ICampaignReadDao campaignReadDao, IErrorHandler<GetCampaignsHandlerAsync> errorHandler, IDynamicQueryBuilderService<Campaign> dynamicQueryBuilderService)
        {
            _campaignReadDao = campaignReadDao;
            _errorHandler = errorHandler;
            _dynamicQueryBuilderService = dynamicQueryBuilderService;
        }

        /// <summary>
        /// 處理 GetCampaignsQuery 並根據指定的篩選條件檢索活動集合。
        /// </summary>
        /// <param name="request">GetCampaignsQuery 請求。</param>
        /// <param name="cancellationToken">取消權杖。</param>
        /// <returns>檢索到的活動集合。</returns>
        public async Task<ICollection<CampaignDto>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<Campaign, bool>> filter = c => true;

                var filters = new List<Expression<Func<Campaign, bool>>>();

                if(request.CampaignId != default)
                    filters.Add(c => c.CampaignId == request.CampaignId);

                else
                {
                    if (request.CampaignName != null)
                        filters.Add(c => c.CampaignName == request.CampaignName);

                    if (request.Location != null)
                        filters.Add(c => c.Location == request.Location);

                    if (request.CampaignStartDate != DateTime.MinValue && request.CampaignEndDate != DateTime.MinValue)
                        filters.Add(c => c.CampaignDate > request.CampaignStartDate && c.CampaignDate < request.CampaignEndDate);

                    if (request.City != null)
                        filters.Add(c => c.City == request.City);
                }

                foreach (var f in filters)
                    filter = _dynamicQueryBuilderService.BuildFilter(filter, f);

                string useCache = filters.Count == 1 && request.CampaignId != default ? $"Campaign:Id:{request.CampaignId}"
                    : filters.Count == 1 && request.City != null ? request.City
                    : filters.Count == 0 ? "Campaign" 
                    : "";

                return await _campaignReadDao.GetCampaignsAsync(filter, useCache);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
