using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketProject.DAL;
using TicketProject.Services.Interfaces;

namespace TicketProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly WriteTicketDbContext _dbContext;
        private readonly IRedisService _redisService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ITicketService _ticketService;

        public DebugController(WriteTicketDbContext dbContext, IRedisService redisService,
            IRabbitMQService rabbitMQService, ITicketService ticketService)
        {
            _dbContext = dbContext;
            _redisService = redisService;
            _rabbitMQService = rabbitMQService;
            _ticketService = ticketService;
        }

        [HttpGet("ResetEnv")]
        public async Task<IResult> ResetEnv([FromQuery] int id = 3002, int quantityAvailable = 2000)
        {
            try
            {
                await RemoveAllOrderAndTicket();
                CleanRedisCache();
                await CleanLog();
                CleanQueue();
                CleanServiceMessages();
                ResetTicketContent(id, quantityAvailable);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }

        [HttpGet("ResetTicketContent")]
        public IResult ResetTicketContent([FromQuery]int id, int quantityAvailable)
        {
            try
            {
                var c = _dbContext.Campaigns
                                  .Include(c => c.TicketContents)
                                  .FirstOrDefault(c => c.CampaignId == id)!;
                
                c.TicketContents.ToList().ForEach(t =>
                {
                    t.QuantitySold = 0;
                    t.QuantityAvailable = quantityAvailable;
                });
                _dbContext.Campaigns.Update(c);
                _dbContext.SaveChanges();
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        [HttpGet("RemoveAllOrderAndTicket")]
        public async Task<IResult> RemoveAllOrderAndTicket()
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM Tickets");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM OrderItems");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM Orders");
                await transaction.CommitAsync();

                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        [HttpGet("CleanRedisCache")]
        public IResult CleanRedisCache()
        {
            try
            {
                _redisService.ClearCacheAsync("", true);
                return Results.Ok();
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        }
        [HttpGet("CleanLog")]
        public async Task<IResult> CleanLog()
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM SystemLog");
                await transaction.CommitAsync();
                return Results.Ok();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return Results.BadRequest(e.Message);
            }
        }
        [HttpGet("CleanQueue")]
        public IResult CleanQueue()
        {
            try
            {
                _rabbitMQService.CleanQueueForDebug();
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        [HttpGet("CleanServiceMessages")]
        public IResult CleanServiceMessages()
        {
            try
            {
                _ticketService.CleanMessagesForDebug();
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
    }
}
