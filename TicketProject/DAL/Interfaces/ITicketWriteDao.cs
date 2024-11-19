using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface ITicketWriteDao
    {
        Task<ICollection<Ticket>> CreateTicketAsync(ICollection<Ticket> tickets);
    }
}
