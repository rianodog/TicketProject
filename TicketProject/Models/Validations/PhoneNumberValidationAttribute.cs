using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TicketProject.Commands;
using TicketProject.DAL.Interfaces;
using TicketProject.Services.Interfaces;

namespace TicketProject.Models.Validations
{
    /// <summary>
    /// 驗證手機號碼是否符合格式並且是否已存在。
    /// </summary>
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// 驗證手機號碼是否符合格式並且是否已存在。
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
                var regex = new Regex(@"^09\d{8}$");

                if (!regex.IsMatch(user!.PhoneNumber!))
                    return new ValidationResult("手機號碼格式不正確。");

                if (_UserReadDao.GetUser(u => u.PhoneNumber == user!.PhoneNumber!) != null)
                    return new ValidationResult("手機號碼已存在。");

                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                var errorHandler = (IErrorHandler<PhoneNumberValidationAttribute>)validationContext
                    .GetRequiredService(typeof(IErrorHandler<PhoneNumberValidationAttribute>));
                errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
