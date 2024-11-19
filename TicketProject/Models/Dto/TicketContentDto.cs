using static TicketProject.Models.Enums;

namespace TicketProject.Models.Dto
{
    public class TicketContentDto
    {
        public int CampaignId { get; set; }
        public int TicketContentId { get; set; }
        public TicketType TypeName { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantitySold { get; set; }
        public decimal Price { get; set; }
        public DateTime UpdateAt { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; } = [];
        public ICollection<TicketDto> Tickets { get; set; } = [];
    }
}
