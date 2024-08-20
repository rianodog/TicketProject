using System.Linq.Expressions;
using TicketProject.Extensions;
using TicketProject.Models.Entity;
using TicketProject.DAL.Interfaces;

namespace TicketProject.DAL.Implement
{
    public class EventDao : IEventDao
    {
        private readonly TicketDbContext _dbContext;
        public EventDao(TicketDbContext ticketDbContext)
        {
            _dbContext = ticketDbContext;
        }

        public Event? GetEvent(int id)
        {
            return _dbContext.Events.Find(id);
        }

        public void AddEvent(Event e)
        {
            _dbContext.Events.Add(e);
            _dbContext.SaveChanges();
        }

        public void UpdateEvent(Event e)
        {
            _dbContext.Events.Update(e);
            _dbContext.SaveChanges();
        }

        public bool DeleteEvent(int id)
        {
            var e = _dbContext.Events.Find(id);
            _dbContext.Events.Remove(e!);
            return _dbContext.SaveChanges() > 0;
        }

        public IEnumerable<Event> GetEvents(Expression<Func<Event, bool>> filter)
        {
            return _dbContext.Events.Where(filter).ToList();
        }

        public IEnumerable<Event> GetEvents()
        {
            return _dbContext.Events.ToList();
        }
    }
}
