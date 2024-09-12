using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    /// <summary>
    /// 代表用於讀取活動資料的資料存取物件。
    /// </summary>
    public class CampaignReadDao : ICampaignReadDao
    {
        private readonly IRedisService _redisService;
        private readonly ReadTicketDbContext _dbContext;
        private readonly IErrorHandler<CampaignReadDao> _errorHandler;

        /// <summary>
        /// 初始化 <see cref="CampaignReadDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="dbContext">資料庫內容。</param>
        /// <param name="errorHandler">錯誤處理程式。</param>
        public CampaignReadDao(ReadTicketDbContext dbContext, IErrorHandler<CampaignReadDao> errorHandler, IRedisService redisService)
        {
            _dbContext = dbContext;
            _errorHandler = errorHandler;
            _redisService = redisService;
        }

        /// <summary>
        /// 根據指定的篩選條件，非同步取得活動清單。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>表示非同步作業的工作物件，工作物件的結果包含活動清單。</returns>
        public async Task<List<Campaign>> GetCampaignAsync(Expression<Func<Campaign, bool>> filter, string useCache)
        {
            try
            {
                if (!String.IsNullOrEmpty(useCache))
                {
                    if (useCache != "Campaigns")
                        useCache = $"Campaigns:City:{useCache}";

                    var result = await _redisService.GetCacheAsync<List<Campaign>>(useCache);

                    if (result == null || result.Count == 0)
                    {
                        result = await _dbContext.Campaigns.Where(filter).ToListAsync();

                        if(result.Count != 0)
                            await _redisService.SetCacheAsync(useCache, result);
                    }
                    return result;
                }
                return await _dbContext.Campaigns.Where(filter).ToListAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
