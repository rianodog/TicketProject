using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    /// <summary>
    /// 實現 ITicketWriteDao 介面的類別，用於處理票務寫入操作。
    /// </summary>
    public class TicketWriteDao : ITicketWriteDao
    {
        private readonly WriteTicketDbContext _writeTicketDbContext;
        private readonly IErrorHandler<TicketWriteDao> _errorHandler;

        /// <summary>
        /// 初始化 <see cref="TicketWriteDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="writeTicketDbContext">寫入資料庫上下文。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public TicketWriteDao(WriteTicketDbContext writeTicketDbContext, IErrorHandler<TicketWriteDao> errorHandler)
        {
            _writeTicketDbContext = writeTicketDbContext;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 非同步地創建多張票。
        /// </summary>
        /// <param name="tickets">要創建的票列表。</param>
        /// <returns>創建的票列表。</returns>
        public async Task<List<Ticket>> CreateTicketsAsync(List<Ticket> tickets)
        {
            try
            {
                _writeTicketDbContext.Tickets.AddRange(tickets);
                await _writeTicketDbContext.SaveChangesAsync();
                return tickets;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
