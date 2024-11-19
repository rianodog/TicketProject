using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    public class TicketContentReadDao : ITicketContentReadDao
    {
        private readonly ReadTicketDbContext _dbContext;
        private readonly IErrorHandler<TicketContentReadDao> _errorHandler;

        public TicketContentReadDao(ReadTicketDbContext dbContext, IErrorHandler<TicketContentReadDao> errorHandler)
        {
            _dbContext = dbContext;
            _errorHandler = errorHandler;
        }

        public async Task<ICollection<TicketContent>?> GetTicketContentsAsync(Expression<Func<TicketContent, bool>> filter)
        {
            try
            {
                return await _dbContext.TicketContents.Where(filter).ToListAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
