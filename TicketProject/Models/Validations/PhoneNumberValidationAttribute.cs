using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Models.Validations
{
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                var _userService = (IUserService)validationContext.GetRequiredService(typeof(IUserService));
                var user = validationContext.ObjectInstance as User;
                var regex = new Regex(@"^09\d{8}$");

                if (!regex.IsMatch(user!.PhoneNumber!))
                    return new ValidationResult("Phone number has the wrong format.");

                if (_userService.GetUserByPhoneNumber(user!.PhoneNumber!) != null)
                    return new ValidationResult("Phone number already exists.");

                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                var errorHandler = (IErrorHandler)validationContext.GetRequiredService(typeof(IErrorHandler));
                var logger = (ILogger<PhoneNumberValidationAttribute>)validationContext.GetRequiredService(typeof(ILogger<PhoneNumberValidationAttribute>));
                errorHandler.HandleError(e, logger, typeof(PhoneNumberValidationAttribute).FullName!);
                throw;
            }
        }
    }
}
