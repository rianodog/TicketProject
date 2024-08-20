using System.ComponentModel.DataAnnotations;

namespace TicketProject.Models.Dto
{
    public class LoginUserDto
    {
        [Required]
        public string? Account { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
