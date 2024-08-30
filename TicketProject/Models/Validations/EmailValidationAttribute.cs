using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TicketProject.Commands;
using TicketProject.DAL.Interfaces;
using TicketProject.Services.Interfaces;

namespace TicketProject.Models.Validations
{
    /// <summary>
    /// 自訂的驗證屬性，用於驗證郵件地址的格式和唯一性。
    /// </summary>
    public class EmailValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// 驗證屬性的有效性。
        /// </summary>
        /// <param name="value">要驗證的值。</param>
        /// <param name="validationContext">驗證的上下文。</param>
        /// <returns>驗證結果。</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                var _UserReadDao = (IUserReadDao)validationContext.GetRequiredService(typeof(IUserReadDao));
                var user = validationContext.ObjectInstance as RegisterCommand;
                var regex = new Regex(@"^.+@.+\..+$");

                if (!regex.IsMatch(user!.Email!))
                    return new ValidationResult("Email has the wrong format.");

                if (_UserReadDao.GetUser(u => u.Email == user!.Email!) != null)
                    return new ValidationResult("Email already exists.");

                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                var errorHandler = (IErrorHandler<EmailValidationAttribute>)validationContext
                    .GetRequiredService(typeof(IErrorHandler<EmailValidationAttribute>));
                errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
