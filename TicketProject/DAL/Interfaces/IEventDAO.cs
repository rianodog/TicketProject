using System.Linq.Expressions;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface IEventDao
    {
        Event? GetEvent(int id);
        void AddEvent(Event e);
        void UpdateEvent(Event e);
        bool DeleteEvent(int id);
        IEnumerable<Event> GetEvents(Expression<Func<Event, bool>> filter);
        IEnumerable<Event> GetEvents();
    }
}
