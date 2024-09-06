using System.Linq.Expressions;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface ICampaignReadDao
    {
        public Task<Campaign?> GetCampaignAsync(Expression<Func<Campaign, bool>> filter);
        public Task<List<Campaign>> GetCampaignsAsync(Expression<Func<Campaign, bool>> filter);
        public Task<List<Campaign>> GetAllCampaignAsync();
    }
}
