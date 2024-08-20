using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Models.Validations
{
    public class EmailValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {   
            try
            {
                var _userService = (IUserService)validationContext.GetRequiredService(typeof(IUserService));
                var user = validationContext.ObjectInstance as User;
                var regex = new Regex(@"^.+@.+\..+$");

                if (!regex.IsMatch(user!.Email!))
                    return new ValidationResult("Email has the wrong format.");
                if (_userService.GetUserByEmail(user!.Email!) != null)
                    return new ValidationResult("Email already exists.");

                return ValidationResult.Success;
            }
            catch(Exception e)
            {
                var errorHandler = (IErrorHandler)validationContext.GetRequiredService(typeof(IErrorHandler));
                var logger = (ILogger<EmailValidationAttribute>)validationContext.GetRequiredService(typeof(ILogger<EmailValidationAttribute>));
                errorHandler.HandleError(e, logger, typeof(EmailValidationAttribute).FullName!);
                throw;
            }
        }
    }
}
