using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketProject.Commands;
using TicketProject.Querys;
using TicketProject.Services.Interfaces;

namespace TicketProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IErrorHandler<UserController> _errorHandler;
        private readonly IJWTService _jwtService;

        /// <summary>  
        /// 初始化 <see cref="UserController"/> 類別的新執行個體。  
        /// </summary>  
        /// <param name="mediator">MediatR 介面。</param>  
        /// <param name="errorHandler">錯誤處理器。</param>  
        /// <param name="jwtService">JWT 服務。</param>  
        public UserController(IMediator mediator, IErrorHandler<UserController> errorHandler, IJWTService jwtService)
        {
            _mediator = mediator;
            _errorHandler = errorHandler;
            _jwtService = jwtService;
        }

        /// <summary>  
        /// 註冊新使用者。  
        /// </summary>  
        /// <param name="registerCommand">註冊命令。</param>  
        /// <returns>註冊結果。</returns>  
        [HttpPost("Register")]
        public async Task<IResult> Register([FromBody] RegisterCommand registerCommand)
        {
            try
            {
                var user = await _mediator.Send(registerCommand);
                return Results.Ok(_jwtService.GenerateJwtToken(user));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                return Results.StatusCode(500);
            }
        }

        /// <summary>  
        /// 使用者登入。  
        /// </summary>  
        /// <param name="loginQuerie">登入查詢。</param>  
        /// <returns>登入結果。</returns>  
        [HttpPost("Login")]
        public async Task<IResult> Login([FromBody] LoginQuery loginQuerie)
        {
            try
            {
                var user = await _mediator.Send(loginQuerie);
                if (user == null)
                    return Results.BadRequest("帳號或密碼不正確。");
                return Results.Ok(_jwtService.GenerateJwtToken(user));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                return Results.StatusCode(500);
            }
        }

        /// <summary>  
        /// 刷新令牌。  
        /// </summary>  
        /// <returns>刷新令牌結果。</returns>  
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
                _errorHandler.HandleError(e);
                return Results.BadRequest("伺服器錯誤。");
            }
        }
    }
}
