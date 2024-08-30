using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    /// <summary>
    /// 使用者寫入資料訪問物件的實現。
    /// </summary>
    public class UserWriteDao : IUserWriteDao
    {
        private readonly WriteTicketDbContext _dbWriteContext;
        private readonly IErrorHandler<UserWriteDao> _errorHandler;

        /// <summary>
        /// 初始化 <see cref="UserWriteDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="dbWriteContext">寫入資料庫上下文。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public UserWriteDao(WriteTicketDbContext dbWriteContext, IErrorHandler<UserWriteDao> errorHandler)
        {
            _dbWriteContext = dbWriteContext;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 新增使用者。
        /// </summary>
        /// <param name="user">要新增的使用者。</param>
        /// <returns>新增後的使用者。</returns>
        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                _dbWriteContext.Users.Add(user);
                await _dbWriteContext.SaveChangesAsync();
                return user;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 更新使用者。
        /// </summary>
        /// <param name="user">要更新的使用者。</param>
        /// <returns>更新後的使用者。</returns>
        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                _dbWriteContext.Users.Update(user);
                await _dbWriteContext.SaveChangesAsync();
                return user;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 刪除使用者。
        /// </summary>
        /// <param name="user">要刪除的使用者。</param>
        public async Task DeleteUserAsync(User user)
        {
            try
            {
                _dbWriteContext.Users.Remove(user);
                await _dbWriteContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}