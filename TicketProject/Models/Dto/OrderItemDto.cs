namespace TicketProject.Models.Dto
{
    public class OrderItemDto
    {
        public int TicketContentId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public TicketDto Ticket { get; set; } = new();
    }
}
