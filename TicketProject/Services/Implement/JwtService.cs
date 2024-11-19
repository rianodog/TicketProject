using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;
using TicketProject.Models;

namespace TicketProject.Services.Implement
{
    /// <summary>
    /// JWT 服務實現類別。
    /// </summary>
    public class JWTService : IJWTService
    {
        private readonly string _secretKey;
        private readonly IErrorHandler<JWTService> _errorHandler;

        /// <summary>
        /// 初始化 JWTService 類別的新執行個體。
        /// </summary>
        /// <param name="configuration">配置。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public JWTService(IConfiguration configuration, IErrorHandler<JWTService> errorHandler)
        {
            _secretKey = configuration["Jwt:SecretKey"]!;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 為指定的使用者生成 JWT 令牌。
        /// </summary>
        /// <param name="user">使用者。</param>
        /// <returns>JWT 令牌陣列。</returns>
        public string[] GenerateJwtToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = GenerateClaims(user),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                };

                // 產生短期和長期的 JWT 令牌
                var shortToken = tokenHandler.CreateToken(tokenDescriptor);
                tokenDescriptor.Expires = DateTime.UtcNow.AddDays(7);
                var longToken = tokenHandler.CreateToken(tokenDescriptor);
                return [tokenHandler.WriteToken(shortToken), tokenHandler.WriteToken(longToken)];
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 使用指定的令牌刷新 JWT 令牌。
        /// </summary>
        /// <param name="token">令牌。</param>
        /// <returns>JWT 令牌陣列，如果令牌無效則為 null。</returns>
        public string[]? RefreshJwtToken(string token)
        {
            try
            {
                var principal = VerifyToken(token);
                if (principal == null)
                    return null;

                var identity = (ClaimsIdentity)principal!.Identity!;
                var user = new User
                {
                    UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                    UserName = identity.FindFirst(ClaimTypes.Name)!.Value,
                    Email = identity.FindFirst(ClaimTypes.Email)!.Value,
                    IsAdmin = (int)Enum.Parse<Enums.UserRoles>(identity.FindFirst(ClaimTypes.Role)!.Value)
                };
                return GenerateJwtToken(user);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 使用指定的令牌驗證 JWT 令牌。
        /// </summary>
        /// <param name="token">令牌。</param>
        /// <returns>驗證後的聲明主體。</returns>
        private ClaimsPrincipal? VerifyToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 為指定的使用者生成聲明。
        /// </summary>
        /// <param name="user">使用者。</param>
        /// <returns>聲明身份。</returns>
        private ClaimsIdentity GenerateClaims(User user)
        {
            try
            {
                return new ClaimsIdentity(
                new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName!),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.Role, ((Enums.UserRoles)user.IsAdmin!).ToString())
                }, "Bearer"
                );
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
