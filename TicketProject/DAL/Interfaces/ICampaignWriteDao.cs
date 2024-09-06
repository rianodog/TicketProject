using TicketProject.Commands;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    /// <summary>
    /// 定義活動寫入資料訪問物件的介面。
    /// </summary>
    public interface ICampaignWriteDao
    {
        /// <summary>
        /// 非同步地創建新的活動。
        /// </summary>
        /// <param name="Campaign">要創建的活動。</param>
        /// <returns>創建的活動。</returns>
        Task<Campaign> CreateCampaignAsync(Campaign campaign);
    }
}
