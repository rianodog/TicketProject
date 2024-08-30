using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    /// <summary>
    /// 使用者讀取資料訪問物件的實現。
    /// </summary>
    public class UserReadDao : IUserReadDao
    {
        private readonly ReadTicketDbContext _dbReadContext;
        private readonly IErrorHandler<UserReadDao> _errorHandler;

        /// <summary>
        /// 初始化 <see cref="UserReadDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="dbReadContext">讀取資料庫上下文。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public UserReadDao(ReadTicketDbContext dbReadContext, IErrorHandler<UserReadDao> errorHandler)
        {
            _dbReadContext = dbReadContext;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 根據指定的篩選條件取得使用者。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>符合篩選條件的使用者，如果找不到使用者則為 null。</returns>
        public User? GetUser(Expression<Func<User, bool>> filter)
        {
            try
            {
                return _dbReadContext.Users.Where(filter).FirstOrDefault();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 根據指定的篩選條件以非同步方式取得使用者。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>表示非同步操作的工作。工作結果包含符合篩選條件的使用者，如果找不到使用者則為 null。</returns>
        public async Task<User?> GetUserAsync(Expression<Func<User, bool>> filter)
        {
            try
            {
                return await _dbReadContext.Users.Where(filter).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 根據指定的篩選條件以非同步方式取得使用者集合。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>表示非同步操作的工作。工作結果包含符合篩選條件的使用者集合。</returns>
        public async Task<IEnumerable<User>> GetUsersAsync(Expression<Func<User, bool>> filter)
        {
            try
            {
                return await _dbReadContext.Users.Where(filter).ToListAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 以非同步方式取得所有使用者。
        /// </summary>
        /// <returns>表示非同步操作的工作。工作結果包含所有使用者的集合。</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _dbReadContext.Users.ToListAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }
    }
}