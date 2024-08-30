namespace TicketProject.Models.Dto
{
    public class CreateTicketDto
    {
        public string? TicketType { get; set; }
        public decimal? Price { get; set; }
        public int? QuantityAvailable { get; set; }
    }
}
