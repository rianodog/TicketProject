namespace TicketProject.Models.Dto
{
    public class CreateEventDto
    {
        public string? EventName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? EventDate { get; set; }
        public List<CreateTicketDto> Tickets { get; set; } = [];
    }
}
