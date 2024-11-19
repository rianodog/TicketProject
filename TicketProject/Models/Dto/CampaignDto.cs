using static TicketProject.Models.Enums;

namespace TicketProject.Models.Dto
{
    public class CampaignDto
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public City City { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CampaignDate { get; set; }
        public ICollection<TicketContentDto> TicketContents { get; set; } = [];
    }
}
