﻿using MediatR;
using TicketProject.DAL.Interfaces;
using TicketProject.Helpers;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Querys.Handlers
{
    /// <summary>  
    /// 處理使用者登入請求的處理程式。  
    /// </summary>  
    public class LoginHandlerAsync : IRequestHandler<LoginQuery, User?>
    {
        private readonly IErrorHandler<LoginHandlerAsync> _errorHandler;
        private readonly IUserReadDao _userReadDao;

        /// <summary>  
        /// 初始化 LoginHandlerAsync 類別的新執行個體。  
        /// </summary>  
        /// <param name="errorHandler">錯誤處理器。</param>  
        /// <param name="userReadDao">用戶讀取資料訪問物件。</param>  
        public LoginHandlerAsync(IErrorHandler<LoginHandlerAsync> errorHandler, IUserReadDao userReadDao)
        {
            _errorHandler = errorHandler;
            _userReadDao = userReadDao;
        }

        /// <summary>  
        /// 處理登入請求。  
        /// </summary>  
        /// <param name="request">登入請求。</param>  
        /// <param name="cancellationToken">取消標記。</param>  
        /// <returns>返回使用者物件，如果登入失敗則返回 null。</returns>  
        public async Task<User?> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var passwordHash = await HashHelper.HashPassword(request.Password);
                var user = await _userReadDao.GetUserAsync(u => (u.Email == request.Account || u.PhoneNumber == request.Account)
                    && u.PasswordHash == passwordHash);
                return user ?? null;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}