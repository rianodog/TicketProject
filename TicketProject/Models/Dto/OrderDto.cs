using static TicketProject.Models.Enums;

namespace TicketProject.Models.Dto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; } = [];
    }
}
