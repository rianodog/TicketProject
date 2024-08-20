using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketProject.DAL.Interfaces;
using TicketProject.Extensions;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Implement
{
    public class UserDao : IUserDao
    {
        private readonly TicketDbContext _dbContext;
        private readonly ILogger<UserDao> _logger;

        /// <summary>
        /// 使用指定的資料庫內容和日誌記錄器初始化 UserDao 類別的新執行個體。
        /// </summary>
        /// <param name="dbContext">資料庫內容。</param>
        /// <param name="logger">日誌記錄器。</param>
        public UserDao(TicketDbContext dbContext, ILogger<UserDao> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// 根據指定的篩選條件，取得符合條件的使用者。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>符合條件的使用者。</returns>
        public async Task<User?> GetUser(Expression<Func<User, bool>> filter)
        {
            try
            {
                return await _dbContext.Users.Where(filter).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                e.Source = typeof(UserDao).FullName;
                _logger.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 根據指定的篩選條件，取得符合條件的使用者。
        /// </summary>
        /// <param name="filter">篩選條件。</param>
        /// <returns>符合條件的使用者。</returns>
        public async Task<IEnumerable<User>> GetUsers(Expression<Func<User, bool>> filter)
        {
            try
            {
                return await _dbContext.Users.Where(filter).ToListAsync();
            }
            catch (Exception e)
            {
                e.Source = typeof(UserDao).FullName;
                _logger.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 取得所有使用者。
        /// </summary>
        /// <returns>所有使用者。</returns>
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                return await _dbContext.Users.ToListAsync();
            }
            catch (Exception e)
            {
                e.Source = typeof(UserDao).FullName;
                _logger.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 新增使用者。
        /// </summary>
        /// <param name="user">要新增的使用者。</param>
        /// <returns>新增後的使用者。</returns>
        public async Task<User> AddUser(User user)
        {
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception e)
            {
                e.Source = typeof(UserDao).FullName;
                _logger.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 更新使用者。
        /// </summary>
        /// <param name="user">要更新的使用者。</param>
        /// <returns>更新後的使用者。</returns>
        public async Task<User> UpdateUser(User user)
        {
            try
            {
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception e)
            {
                e.Source = typeof(UserDao).FullName;
                _logger.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 刪除使用者。
        /// </summary>
        /// <param name="user">要刪除的使用者。</param>
        public async Task DeleteUser(User user)
        {
            try
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                e.Source = typeof(UserDao).FullName;
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}