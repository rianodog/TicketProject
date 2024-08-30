using TicketProject.Commands;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    /// <summary>
    /// 實現 IEventWriteDao 介面的類別，用於處理事件寫入操作。
    /// </summary>
    public class EventWriteDao : IEventWriteDao
    {
        private readonly WriteTicketDbContext _writeTicketDbContext;
        private readonly IErrorHandler<EventWriteDao> _errorHandler;

        /// <summary>
        /// 初始化 <see cref="EventWriteDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="writeTicketDbContext">寫入資料庫上下文。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public EventWriteDao(WriteTicketDbContext writeTicketDbContext, IErrorHandler<EventWriteDao> errorHandler)
        {
            _writeTicketDbContext = writeTicketDbContext;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 非同步地創建一個新的事件。
        /// </summary>
        /// <param name="event">要創建的事件實體。</param>
        /// <returns>創建的事件實體。</returns>
        public async Task<Event> CreateEventAsync(Event @event)
        {
            try
            {
                _writeTicketDbContext.Events.Add(@event);
                await _writeTicketDbContext.SaveChangesAsync();
                return @event;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
