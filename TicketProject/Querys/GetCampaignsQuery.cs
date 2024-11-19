using MediatR;
using TicketProject.Models.Dto;

namespace TicketProject.Querys
{
    public class GetCampaignsQuery : IRequest<ICollection<CampaignDto>>
    {
        public int CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public string? City { get; set; }
        public string? Location { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public DateTime CampaignEndDate { get; set; }
    }
}
