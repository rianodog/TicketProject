using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;
using TicketProject.Extensions;

namespace TicketProject.Services.Implement
{
    public class JWTService : IJwtService
    {
        private readonly string _secretKey;
        private readonly ILogger<JWTService> _logger;
        private readonly IErrorHandler _errorHandler;

        public JWTService(IConfiguration configuration, ILogger<JWTService> logger, IErrorHandler errorHandler)
        {
            _secretKey = configuration["Jwt:SecretKey"]!;
            _logger = logger;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 為指定的使用者生成聲明。
        /// </summary>
        /// <param name="user">使用者。</param>
        /// <returns>聲明身份。</returns>
        public ClaimsIdentity GenerateClaims(User user)
        {
            try
            {
                return new ClaimsIdentity(
                [
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                                new Claim(ClaimTypes.Name, user.Username!),
                                new Claim(ClaimTypes.Email, user.Email!),
                                new Claim(ClaimTypes.Role, ((Enums.UserRoles)user.IsAdmin!).ToString())
                ], "Bearer"
                );
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(JWTService).FullName!);
                throw;
            }
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
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                };

                // 產生短期和長期的 JWT 令牌
                var shortToken = tokenHandler.CreateToken(tokenDescriptor);
                tokenDescriptor.Expires = DateTime.Now.AddDays(7);
                var longToken = tokenHandler.CreateToken(tokenDescriptor);
                return [tokenHandler.WriteToken(shortToken), tokenHandler.WriteToken(longToken)];
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(JWTService).FullName!);
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

                var identity = (ClaimsIdentity)principal!.Identity!;
                var user = new User
                {
                    UserId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                    Username = identity.FindFirst(ClaimTypes.Name)!.Value,
                    Email = identity.FindFirst(ClaimTypes.Email)!.Value,
                    IsAdmin = (int)Enum.Parse<Enums.UserRoles>(identity.FindFirst(ClaimTypes.Role)!.Value)
                };
                return GenerateJwtToken(user);
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(JWTService).FullName!);
                throw;
            }
        }

        /// <summary>
        /// 使用指定的令牌驗證 JWT 令牌。
        /// </summary>
        /// <param name="token">令牌。</param>
        /// <returns>驗證後的聲明主體。</returns>
        public ClaimsPrincipal VerifyToken(string token)
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
                throw;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(JWTService).FullName!);
                throw;
            }
        }
    }
}
