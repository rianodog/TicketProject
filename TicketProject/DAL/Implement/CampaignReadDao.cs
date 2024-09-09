using Microsoft.EntityFrameworkCore;
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
        private readonly ReadTicketDbContext _dbContext;
        private readonly IErrorHandler<CampaignReadDao> _errorHandler;

        /// <summary>
        /// 初始化 <see cref="CampaignReadDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="dbContext">資料庫內容。</param>
        /// <param name="errorHandler">錯誤處理程式。</param>
        public CampaignReadDao(ReadTicketDbContext dbContext, IErrorHandler<CampaignReadDao> errorHandler)
        {
            _dbContext = dbContext;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 非同步取得所有活動。
        /// </summary>
        /// <returns>表示非同步作業的工作物件，工作物件的結果包含活動清單。</returns>
        public async Task<List<Campaign>> GetAllCampaignAsync()
        {
            try
            {
                return await _dbContext.Campaigns.ToListAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 根據指定的篩選條件，非同步取得活動。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>表示非同步作業的工作物件，工作物件的結果包含活動，如果找不到則為 null。</returns>
        public async Task<Campaign?> GetCampaignAsync(Expression<Func<Campaign, bool>> filter)
        {
            try
            {
                return await _dbContext.Campaigns.FirstOrDefaultAsync(filter);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 根據指定的篩選條件，非同步取得活動清單。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>表示非同步作業的工作物件，工作物件的結果包含活動清單。</returns>
        public async Task<List<Campaign>> GetCampaignsAsync(Expression<Func<Campaign, bool>> filter)
        {
            try
            {
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
