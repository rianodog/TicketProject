using MediatR;
using System.ComponentModel.DataAnnotations;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

namespace TicketProject.Commands
{
    public class CreateCampaignCommand : IRequest<CreateCampaignCommand>
    {
        [Required]
        public string CampaignName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public DateTime CampaignDate { get; set; }
        [Required]
        public ICollection<CreateCampaign_TicketContentDto> TicketContents { get; set; } = [];
    }
}
