using System.Linq.Expressions;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    /// <summary>
    /// 提供使用者資料存取的介面。
    /// </summary>
    public interface IUserDao
    {
        /// <summary>
        /// 根據條件過濾取得單一使用者。
        /// </summary>
        /// <param name="filter">過濾條件。</param>
        /// <returns>符合條件的使用者，若無則為 null。</returns>
        Task<User?> GetUser(Expression<Func<User, bool>> filter);

        /// <summary>
        /// 根據條件過濾取得多個使用者。
        /// </summary>
        /// <param name="filter">過濾條件。</param>
        /// <returns>符合條件的使用者集合。</returns>
        Task<IEnumerable<User>> GetUsers(Expression<Func<User, bool>> filter);

        /// <summary>
        /// 取得所有使用者。
        /// </summary>
        /// <returns>所有使用者的集合。</returns>
        Task<IEnumerable<User>> GetAllUsers();

        /// <summary>
        /// 新增使用者。
        /// </summary>
        /// <param name="user">要新增的使用者。</param>
        /// <returns>新增後的使用者。</returns>
        Task<User> AddUser(User user);

        /// <summary>
        /// 更新使用者。
        /// </summary>
        /// <param name="user">要更新的使用者。</param>
        /// <returns>更新後的使用者。</returns>
        Task<User> UpdateUser(User user);

        /// <summary>
        /// 刪除使用者。
        /// </summary>
        /// <param name="user">要刪除的使用者。</param>
        /// <returns>表示異步刪除操作的任務。</returns>
        Task DeleteUser(User user);
    }
}
