using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    public class TicketWriteDao : ITicketWriteDao
    {
        private readonly WriteTicketDbContext _dbContext;
        private readonly IErrorHandler<TicketWriteDao> _errorHandler;

        public TicketWriteDao(WriteTicketDbContext dbContext, IErrorHandler<TicketWriteDao> errorHandler)
        {
            _dbContext = dbContext;
            _errorHandler = errorHandler;
        }

        public async Task<ICollection<Ticket>> CreateTicketAsync(ICollection<Ticket> tickets)
        {
            try
            {
                _dbContext.Tickets.AddRange(tickets);
                await _dbContext.SaveChangesAsync();
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
