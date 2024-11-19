using MediatR;
using System.ComponentModel.DataAnnotations;
using TicketProject.Models.Dto.ButTicket;

namespace TicketProject.Commands
{
    public class BuyTicketCommand : IRequest<bool>
    {
        [Required]
        public int CampaignId { get; set; }
        [Required]
        public ICollection<BuyListDto> BuyList { get; set; } = [];
        public string UserId { get; set; } = string.Empty;
    }
}
