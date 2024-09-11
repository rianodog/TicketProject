using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketProject.Commands;
using TicketProject.Querys;
using TicketProject.Services.Interfaces;

namespace TicketProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IErrorHandler<CampaignController> _errorHandler;

        /// <summary>  
        /// 初始化 <see cref="CampaignController"/> 類別的新執行個體。  
        /// </summary>  
        /// <param name="mediator">MediatR 介面。</param>  
        /// <param name="errorHandler">錯誤處理器。</param>  
        public CampaignController(IMediator mediator, IErrorHandler<CampaignController> errorHandler)
        {
            _mediator = mediator;
            _errorHandler = errorHandler;
        }

        /// <summary>  
        /// 建立活動。  
        /// </summary>  
        /// <param name="createCampaignCommand">建立活動的命令。</param>  
        /// <returns>表示建立活動結果的介面。</returns>  
        [HttpPost("CreateCampaign")]
        public async Task<IResult> CreateCampaign([FromBody] CreateCampaignCommand createCampaignCommand)
        {
            try
            {
                return Results.Ok(await _mediator.Send(createCampaignCommand));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                return Results.StatusCode(500);
            }
        }
        [HttpGet("GetCampaign")]
        public async Task<IResult> GetCampaign([FromQuery] GetCampaignQuery getCampaignQuery)
        {
            try
            {
                return Results.Ok(await _mediator.Send(getCampaignQuery));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                return Results.StatusCode(500);
            }
        }
        [HttpGet("GetCampaigns")]
        public async Task<IResult> GetCampaigns([FromQuery] GetCampaignQuery getCampaignQuery)
        {
            try
            {
                return Results.Ok(await _mediator.Send(getCampaignQuery));
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                return Results.StatusCode(500);
            }
        }
    }
}
