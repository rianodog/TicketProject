using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface ITicketWriteDao
    {
        public Task<List<Ticket>> CreateTicketsAsync(List<Ticket> tickets);
    }
}
