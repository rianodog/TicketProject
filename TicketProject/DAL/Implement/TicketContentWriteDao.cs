using EFCore.BulkExtensions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    public class TicketContentWriteDao : ITicketContentWriteDao
    {
        private readonly WriteTicketDbContext _dbContext;
        private readonly IErrorHandler<TicketContentWriteDao> _errorHandler;

        public TicketContentWriteDao(WriteTicketDbContext writeTicketDbContext, IErrorHandler<TicketContentWriteDao> errorHandler)
        {
            _dbContext = writeTicketDbContext;
            _errorHandler = errorHandler;
        }
        public async Task UpdateTicketsContentsAsync(ICollection<TicketContent> ticketContents)
        {
            try
            {
                await _dbContext.BulkUpdateAsync(ticketContents);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
