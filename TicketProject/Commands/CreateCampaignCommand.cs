using MediatR;
using System.ComponentModel.DataAnnotations;
using TicketProject.Models.Dto;
using static TicketProject.Models.Enums;

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
        [Range(0, 2, ErrorMessage = "城市欄位格式錯誤")]
        public City City { get; set; }
        [Required]
        public DateTime CampaignDate { get; set; }
        [Required]
        public ICollection<CreateCampaign_TicketContentDto> TicketContents { get; set; } = [];
    }
}
