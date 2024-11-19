using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    /// <summary>  
    /// 提供雜湊功能的輔助服務。  
    /// </summary>  
    public class HashService : IHashService
    {
        private readonly IErrorHandler<HashService> _errorHandler;

        public HashService(IErrorHandler<HashService> errorHandler)
        {
            _errorHandler = errorHandler;
        }

        /// <summary>  
        /// 使用 SHA256 演算法對密碼進行雜湊。  
        /// </summary>  
        /// <param name="password">要雜湊的密碼。</param>  
        /// <returns>雜湊後的密碼字串。</returns>  
        /// <exception cref="Exception">當雜湊過程中發生錯誤時拋出。</exception>  
        public async Task<string> BcryptHashPassword(string password)
        {
            try
            {
                return await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(password));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task<bool> VerifyPasswordBcrypt(string password, string hashedPassword)
        {
            try
            {
                return await Task.Run(() => BCrypt.Net.BCrypt.Verify(password, hashedPassword));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
