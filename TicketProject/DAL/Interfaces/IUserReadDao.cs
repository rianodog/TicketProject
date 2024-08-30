using System.Linq.Expressions;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    /// <summary>
    /// 提供使用者資料存取的介面。
    /// </summary>
    public interface IUserReadDao
    {
        /// <summary>
        /// 根據條件過濾取得單一使用者。
        /// </summary>
        /// <param name="filter">過濾條件。</param>
        /// <returns>符合條件的使用者，若無則為 null。</returns>
        Task<User?> GetUserAsync(Expression<Func<User, bool>> filter);

        User? GetUser(Expression<Func<User, bool>> filter);

        /// <summary>
        /// 根據條件過濾取得多個使用者。
        /// </summary>
        /// <param name="filter">過濾條件。</param>
        /// <returns>符合條件的使用者集合。</returns>
        Task<IEnumerable<User>> GetUsersAsync(Expression<Func<User, bool>> filter);

        /// <summary>
        /// 取得所有使用者。
        /// </summary>
        /// <returns>所有使用者的集合。</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
