using MediatR;
using TicketProject.Models.Entity;
using TicketProject.DAL.Interfaces;
using TicketProject.Helpers;
using TicketProject.Services.Interfaces;
using AutoMapper;

namespace TicketProject.Commands.Handlers
{
    /// <summary>  
    /// 處理註冊命令的處理器。  
    /// </summary>  
    public class RegisterHandlerAsync : IRequestHandler<RegisterCommand, User>
    {
        private readonly IUserWriteDao _UserWriteDao;
        private readonly IErrorHandler<RegisterHandlerAsync> _errorHandler;
        private readonly IMapper _mapper;

        /// <summary>  
        /// 初始化 <see cref="RegisterHandlerAsync"/> 類別的新執行個體。  
        /// </summary>  
        /// <param name="userDao">使用者寫入資料訪問物件。</param>  
        /// <param name="errorHandler">錯誤處理器。</param>  
        /// <param name="mapper">AutoMapper 介面。</param>  
        public RegisterHandlerAsync(IUserWriteDao userDao, IErrorHandler<RegisterHandlerAsync> errorHandler, IMapper mapper)
        {
            _UserWriteDao = userDao;
            _errorHandler = errorHandler;
            _mapper = mapper;
        }

        /// <summary>  
        /// 非同步處理註冊命令。  
        /// </summary>  
        /// <param name="request">註冊命令。</param>  
        /// <param name="cancellationToken">取消權杖。</param>  
        /// <returns>註冊後的使用者。</returns>  
        public async Task<User> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.PasswordHash = await HashHelper.HashPassword(request.PasswordHash!);
                var user = _mapper.Map<User>(request);
                return await _UserWriteDao.AddUserAsync(user);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}
