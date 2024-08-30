using AutoMapper;
using MediatR;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Commands.Handlers
{
    /// <summary>  
    /// 處理建立事件命令的處理器。  
    /// </summary>  
    public class CreateEventHandlerAsync : IRequestHandler<CreateEventCommand, CreateEventCommand>
    {
        private readonly IEventWriteDao _eventWriteDao;
        private readonly IErrorHandler<CreateEventHandlerAsync> _errorHandler;
        private readonly IMapper _mapper;

        /// <summary>  
        /// 初始化 <see cref="CreateEventHandlerAsync"/> 類別的新執行個體。  
        /// </summary>  
        /// <param name="eventWriteDao">事件寫入 DAO。</param>  
        /// <param name="errorHandler">錯誤處理程式。</param>  
        /// <param name="mapper">映射程式。</param>  
        public CreateEventHandlerAsync(IEventWriteDao eventWriteDao, IErrorHandler<CreateEventHandlerAsync> errorHandler, IMapper mapper)
        {
            _eventWriteDao = eventWriteDao;
            _errorHandler = errorHandler;
            _mapper = mapper;
        }

        /// <summary>  
        /// 處理建立事件的命令。  
        /// </summary>  
        /// <param name="request">建立事件的命令。</param>  
        /// <param name="cancellationToken">取消權杖。</param>  
        /// <returns>已建立的事件命令。</returns>  
        public async Task<CreateEventCommand> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 因Ticket Entity的Event欄位是外鍵 因此為必填  
                // 但從流程來說是先創建Event再創建Ticket並連接到Event的一對多關係  
                // 所以在映射規則先忽略Event.Tickets 這樣才能先創建Event  
                // 之後再將Requst.Tickets映射到Event.Tickets  
                var eventEntity = _mapper.Map<Event>(request);
                eventEntity.Tickets = _mapper.Map<List<Ticket>>(request.Tickets);

                eventEntity = await _eventWriteDao.CreateEventAsync(eventEntity);

                // 將Entity轉換為Dto 避免循環參考  
                var eventDto = _mapper.Map<CreateEventCommand>(eventEntity);
                return eventDto;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
