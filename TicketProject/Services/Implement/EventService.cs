using TicketProject.Services.Interfaces;
using TicketProject.DAL;
using TicketProject.Models.Entity;
using TicketProject.DAL.Interfaces;
using System.Linq.Expressions;

namespace TicketProject.Services.Implement
{
    public class EventService : IEventService
    {
        private readonly IEventDao _eventDAO;

        public EventService(IEventDao eventDAO)
        {
            _eventDAO = eventDAO;
        }

        public Event? GetEventById(int id)
        {
            return _eventDAO.GetEvent(id);
        }

        public void AddEvent(Event e)
        {
            _eventDAO.AddEvent(e);
        }

        public void UpdateEvent(Event e)
        {
            _eventDAO.UpdateEvent(e);
        }

        public void DeleteEvent(int id)
        {
            _eventDAO.DeleteEvent(id);
        }

        public IEnumerable<Event> GetEvents(Expression<Func<Event, bool>> filter)
        {
            return _eventDAO.GetEvents(filter);
        }

        public IEnumerable<Event> GetEvents()
        {
            return _eventDAO.GetEvents();
        }
    }
}
