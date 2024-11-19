using TicketProject.Commands;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface ITicketContentWriteDao
    {
        Task UpdateTicketsContentsAsync(ICollection<TicketContent> ticketContents);
    }
}
