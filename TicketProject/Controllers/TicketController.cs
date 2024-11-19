using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketProject.Commands;
using TicketProject.Services.Interfaces;

namespace TicketProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IErrorHandler<TicketController> _errorHandler;

        public TicketController(IMediator mediator, IErrorHandler<TicketController> errorHandler)
        {
            _mediator = mediator;
            _errorHandler = errorHandler;
        }

        [Authorize]
        [HttpPost]
        public async Task<IResult> BuyTicket([FromBody]BuyTicketCommand buyTicketCommand)
        {
            try
            {
                buyTicketCommand.UserId = User.Claims.FirstOrDefault(c => new Uri(c.Type).Segments.Last() == "nameidentifier")!.Value!;

                var result = await _mediator.Send(buyTicketCommand);
                return Results.Ok(result);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
    }
}
