using System.ComponentModel.DataAnnotations;
using TicketProject.Models.Entity;
using TicketProject.Models.Validations;

namespace TicketProject.Models.Dto
{
    public class RegisterUserDto : User
    {
        [Required]
        [EmailValidation]
        public new string? Email
        {
            get => base.Email;//由於使用ORM 因此實際數據需要讀取以及寫回基類(Entity)
            set => base.Email = value;
        }
        [Required]
        [PhoneNumberValidation]
        public new string? PhoneNumber
        {
            get => base.PhoneNumber;
            set => base.PhoneNumber = value;
        }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,12}$", ErrorMessage = "Password must be 6-12 characters long and include at least one uppercase letter, one lowercase letter, and one number.")]
        public new string? PasswordHash
        {
            get => base.PasswordHash; 
            set => base.PasswordHash = value;
        }
    }
}
