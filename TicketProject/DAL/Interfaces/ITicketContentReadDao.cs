using System.Linq.Expressions;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface ITicketContentReadDao
    {
        Task<ICollection<TicketContent>?> GetTicketContentsAsync(Expression<Func<TicketContent, bool>> filter);
    }
}
