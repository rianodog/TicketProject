using TicketProject.Services.Interfaces;

namespace TicketProject.Factory.Interfaces
{
    public interface IRedisServiceFactory
    {
        IRedisService Create();
    }
}
