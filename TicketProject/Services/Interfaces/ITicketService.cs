using TicketProject.Models.Entity;

namespace TicketProject.Services.Interfaces
{
    public interface ITicketService
    {
        void CreateTicket(Ticket ticket);

        void UpdateTicket(Ticket ticket);

        void DeleteTicket(int id);

        IEnumerable<Ticket> GetAllTickets();

        Ticket GetTicketById(int id);
    }
}
