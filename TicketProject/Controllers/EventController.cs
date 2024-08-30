using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketProject.Commands;
using TicketProject.Services.Interfaces;

namespace TicketProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IErrorHandler<EventController> _errorHandler;

        /// <summary>  
        /// 初始化 <see cref="EventController"/> 類別的新執行個體。  
        /// </summary>  
        /// <param name="mediator">MediatR 介面。</param>  
        /// <param name="errorHandler">錯誤處理器。</param>  
        public EventController(IMediator mediator, IErrorHandler<EventController> errorHandler)
        {
            _mediator = mediator;
            _errorHandler = errorHandler;
        }

        /// <summary>  
        /// 建立事件。  
        /// </summary>  
        /// <param name="createEventCommand">建立事件的命令。</param>  
        /// <returns>表示建立事件結果的介面。</returns>  
        [HttpPost("CreateEvent")]
        public async Task<IResult> CreateEvent([FromBody] CreateEventCommand createEventCommand)
        {
            try
            {
                return Results.Ok(await _mediator.Send(createEventCommand));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                return Results.StatusCode(500);
            }
        }
    }
}
