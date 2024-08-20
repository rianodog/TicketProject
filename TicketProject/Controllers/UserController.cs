using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TicketProject.Models.Dto;
using TicketProject.Services.Interfaces;

namespace TicketProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        public UserController(ILogger<UserController> logger, IUserService userService, IJwtService jwtService)
        {
            _logger = logger;
            _userService = userService;
            _jwtService = jwtService;
        }
        [HttpPost("Register")]
        public async Task<IResult> Register([FromBody]RegisterUserDto registerUserDto)
        {
            try
            {
                var user = await _userService.RegisterUser(registerUserDto);
                var tokens = _jwtService.GenerateJwtToken(user);
                return Results.Ok(tokens);
            }
            catch (Exception e)
            {
                if(string.IsNullOrEmpty(e.Source))
                {
                    e.Source = typeof(UserController).FullName;
                    _logger.LogError(e.Message);
                }
                return Results.BadRequest("Server error.");
            }
        }

        [Authorize]
        [HttpGet("RefreshToken")]
        public IResult RefreshToken()
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
                var token = authorizationHeader.Split(' ')[1];
                var tokens = _jwtService.RefreshJwtToken(token);
                if (tokens == null)
                    return Results.Unauthorized();

                return Results.Ok(tokens);
            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(e.Source))
                {
                    e.Source = typeof(UserController).FullName;
                    _logger.LogError(e.Message);
                }
                return Results.BadRequest("Server error.");
            }
        }
        [HttpPost("Login")]
        public IResult Login([FromBody]LoginUserDto loginUserDto)
        {
            using var rng = new RNGCryptoServiceProvider();
            var key = new byte[256];
            rng.GetBytes(key);
            return Results.Ok(Convert.ToBase64String(key));
        }
    }
}
