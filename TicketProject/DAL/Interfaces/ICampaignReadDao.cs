using System.Linq.Expressions;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface ICampaignReadDao
    {
        Task<ICollection<CampaignDto>> GetCampaignsAsync(Expression<Func<Campaign, bool>> filter, string useCache);
        Task<CampaignDto> GetCampgignFormIdAsync(int id);
    }
}
