using TicketProject.Models.Entity;

namespace TicketProject.Services.Interfaces
{
    /// <summary>  
    /// 定義 JWT 服務介面。  
    /// </summary>  
    public interface IJWTService
    {
        /// <summary>  
        /// 生成 JWT 令牌。  
        /// </summary>  
        /// <param name="user">用戶資訊。</param>  
        /// <returns>包含 JWT 令牌的字串陣列。</returns>  
        public string[] GenerateJwtToken(User user);

        /// <summary>  
        /// 刷新 JWT 令牌。  
        /// </summary>  
        /// <param name="token">現有的 JWT 令牌。</param>  
        /// <returns>包含新的 JWT 令牌的字串陣列，如果刷新失敗則返回 null。</returns>  
        public string[]? RefreshJwtToken(string token);
    }
}
