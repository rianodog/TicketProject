using System.Linq.Expressions;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    /// <summary>
    /// 提供使用者資料存取的介面。
    /// </summary>
    public interface IUserWriteDao
    {
        /// <summary>
        /// 新增使用者。
        /// </summary>
        /// <param name="user">要新增的使用者。</param>
        /// <returns>新增後的使用者。</returns>
        Task<User> AddUserAsync(User user);

        /// <summary>
        /// 更新使用者。
        /// </summary>
        /// <param name="user">要更新的使用者。</param>
        /// <returns>更新後的使用者。</returns>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// 刪除使用者。
        /// </summary>
        /// <param name="user">要刪除的使用者。</param>
        /// <returns>表示異步刪除操作的任務。</returns>
        Task DeleteUserAsync(User user);
    }
}
