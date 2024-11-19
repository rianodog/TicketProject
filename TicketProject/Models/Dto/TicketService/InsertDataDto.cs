namespace TicketProject.Models.Dto.TicketService
{
    public class InsertDataDto
    {
        public OrderDto Order { get; set; } = new OrderDto();
        public List<UpdateTicketContentDto> UpdateTicketContentDtos { get; set; } = [];
    }

    public class UpdateTicketContentDto
    {
        public TicketContentDto TicketContentDto { get; set; } = new();
        public int Quantity { get; set; }
    }
}
