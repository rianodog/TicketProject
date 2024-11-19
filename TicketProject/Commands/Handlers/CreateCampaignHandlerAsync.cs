using AutoMapper;
using MediatR;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Commands.Handlers
{
    /// <summary>  
    /// 處理建立活動命令的處理器。  
    /// </summary>  
    public class CreateCampaignHandlerAsync : IRequestHandler<CreateCampaignCommand, CreateCampaignCommand>
    {
        private readonly ICampaignWriteDao _campaignWriteDao;
        private readonly IErrorHandler<CreateCampaignHandlerAsync> _errorHandler;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        /// <summary>  
        /// 初始化 <see cref="CreateCampaignHandlerAsync"/> 類別的新執行個體。  
        /// </summary>  
        /// <param name="campaignWriteDao">活動寫入 DAO。</param>  
        /// <param name="errorHandler">錯誤處理程式。</param>  
        /// <param name="mapper">映射程式。</param>  
        public CreateCampaignHandlerAsync(ICampaignWriteDao campaignWriteDao, IErrorHandler<CreateCampaignHandlerAsync> errorHandler, IMapper mapper, IRedisService redisService)
        {
            _campaignWriteDao = campaignWriteDao;
            _errorHandler = errorHandler;
            _mapper = mapper;
            _redisService = redisService;
        }

        /// <summary>  
        /// 處理建立活動的命令。  
        /// </summary>  
        /// <param name="request">建立活動的命令。</param>  
        /// <param name="cancellationToken">取消權杖。</param>  
        /// <returns>已建立的活動命令。</returns>  
        public async Task<CreateCampaignCommand> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var campaign = _mapper.Map<Campaign>(request);
                var result =  await _campaignWriteDao.CreateCampaignAsync(campaign);
                await _redisService.ClearCacheAsync("Campaign", true);
                return _mapper.Map<CreateCampaignCommand>(result);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
