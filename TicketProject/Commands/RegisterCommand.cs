using MediatR;
using System.ComponentModel.DataAnnotations;
using TicketProject.Models.Entity;
using TicketProject.Models.Validations;

namespace TicketProject.Commands
{
    public class RegisterCommand : IRequest<User>
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailValidation]
        public string? Email { get; set; }
        [Required]
        [PhoneNumberValidation]
        public string? PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,12}$", ErrorMessage = "Password must be 6-12 characters long and include at least one uppercase letter, one lowercase letter, and one number.")]
        public string? PasswordHash { get; set; }
    }
}
