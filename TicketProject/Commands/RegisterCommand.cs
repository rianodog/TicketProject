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
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,12}$", ErrorMessage = "密碼必須為6-12個字符，並包含至少一個大寫字母、一個小寫字母和一個數字。")]
        public string? PasswordHash { get; set; }
    }
}
