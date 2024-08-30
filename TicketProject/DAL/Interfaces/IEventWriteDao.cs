using TicketProject.Commands;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    /// <summary>
    /// 定義事件寫入資料訪問物件的介面。
    /// </summary>
    public interface IEventWriteDao
    {
        /// <summary>
        /// 非同步地創建新的事件。
        /// </summary>
        /// <param name="event">要創建的事件。</param>
        /// <returns>創建的事件。</returns>
        Task<Event> CreateEventAsync(Event @event);
    }
}
