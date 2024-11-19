using static TicketProject.Models.Enums;

namespace TicketProject.Models.Dto.ButTicket
{
    public class BuyListDto
    {
        public int Quantity { get; set; }
        public TicketType TypeName { get; set; }
    }
}
