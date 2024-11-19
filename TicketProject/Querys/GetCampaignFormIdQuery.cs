using MediatR;
using TicketProject.Models.Dto;

namespace TicketProject.Querys
{
    public class GetCampaignFormIdQuery : IRequest<CampaignDto>
    {
        public int CampaignId { get; set; }
    }
}
