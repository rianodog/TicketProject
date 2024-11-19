using TicketProject.Services.Interfaces;

namespace TicketProject.Factory.Interfaces
{
    public interface IRabbitMQServiceFactory
    {
        IRabbitMQService Create();
    }
}
