using MediatR;
using TicketProject.Models.Entity;

namespace TicketProject.Querys
{
    public class LoginQuery : IRequest<User?>
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
