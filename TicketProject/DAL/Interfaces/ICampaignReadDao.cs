using System.Linq.Expressions;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface ICampaignReadDao
    {
        Task<List<Campaign>> GetCampaignAsync(Expression<Func<Campaign, bool>> filter, string useCache);
    }
}
