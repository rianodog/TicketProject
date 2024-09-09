using MediatR;
using TicketProject.Models.Entity;

namespace TicketProject.Querys
{
    public class GetCampaignsQuery : IRequest<ICollection<Campaign>>
    {
        public string? CampaignName { get; set; }
        public string? Location { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public DateTime CampaignEndDate { get; set; }
    }
}
