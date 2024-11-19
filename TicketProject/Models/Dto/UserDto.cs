namespace TicketProject.Models.Dto
{
    public class UserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int IsAdmin { get; set; }
        public ICollection<OrderDto> Orders { get; set; } = [];
        public ICollection<TicketDto> Tickets { get; set; } = [];
    }
}
