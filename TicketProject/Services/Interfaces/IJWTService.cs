using System.Security.Claims;
using TicketProject.Models.Entity;

namespace TicketProject.Services.Interfaces
{
    public interface IJWTService
    {
        public ClaimsIdentity GenerateClaims(User user);
        public string[] GenerateJwtToken(User user);
        public ClaimsPrincipal? VerifyToken(string token);
        public string[]? RefreshJwtToken(string token);
    }
}
