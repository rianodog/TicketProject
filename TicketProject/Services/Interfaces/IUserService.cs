using System.Linq.Expressions;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

namespace TicketProject.Services.Interfaces
{
    public interface IUserService
    {
        User? GetUserByPhoneNumber(string phoneNumber);

        User? GetUserByEmail(string email);

        Task<User?> GetUserByLogin(LoginUserDto loginUserDto);

        Task<IEnumerable<User>> GeUsersByFilter(Expression<Func<User, bool>> filter);

        Task<IEnumerable<User>> GetAllUsers();

        Task<User> RegisterUser(User user);

        Task<User> UpdateUser(User user);

        Task DeleteUser(int id);

    }
}
