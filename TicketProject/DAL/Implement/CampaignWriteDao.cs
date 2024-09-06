using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    /// <summary>
    /// 實現 ICampaignWriteDao 介面的類別，用於處理活動寫入操作。
    /// </summary>
    public class CampaignWriteDao : ICampaignWriteDao
    {
        private readonly WriteTicketDbContext _writeTicketDbContext;
        private readonly IErrorHandler<CampaignWriteDao> _errorHandler;

        /// <summary>
        /// 初始化 <see cref="CampaignWriteDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="writeTicketDbContext">寫入資料庫上下文。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public CampaignWriteDao(WriteTicketDbContext writeTicketDbContext, IErrorHandler<CampaignWriteDao> errorHandler)
        {
            _writeTicketDbContext = writeTicketDbContext;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 非同步地創建一個新的活動。
        /// </summary>
        /// <param name="campaign">要創建的活動實體。</param>
        /// <returns>創建的活動實體。</returns>
        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            try
            {
                _writeTicketDbContext.Campaigns.Add(campaign);
                await _writeTicketDbContext.SaveChangesAsync();
                return campaign;
            }   
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
