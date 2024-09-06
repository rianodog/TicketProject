using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    public class CampaignReadDao : ICampaignReadDao
    {
        private readonly ReadTicketDbContext _dbContext;
        private readonly IErrorHandler<CampaignReadDao> _errorHandler;

        public CampaignReadDao(ReadTicketDbContext dbContext, IErrorHandler<CampaignReadDao> errorHandler)
        {
            _dbContext = dbContext;
            _errorHandler = errorHandler;
        }

        public async Task<List<Campaign>> GetAllCampaignAsync()
        {
            try
            {
                return await _dbContext.Campaigns.ToListAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task<Campaign?> GetCampaignAsync(Expression<Func<Campaign, bool>> filter)
        {
            try
            {
                return await _dbContext.Campaigns.FirstOrDefaultAsync(filter);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        public async Task<List<Campaign>> GetCampaignsAsync(Expression<Func<Campaign, bool>> filter)
        {
            try
            {
                return await _dbContext.Campaigns.Where(filter).ToListAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
