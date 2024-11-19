using TicketProject.Models.Dto.TicketService;

namespace TicketProject.Services.Interfaces
{
    public interface ITicketService
    {
        //Task InsertToQueue(InsertDataDto insertDataDto);
        void CleanMessagesForDebug();
    }
}
