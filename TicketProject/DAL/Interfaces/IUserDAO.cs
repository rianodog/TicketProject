using TicketProject.Models.DTO;
using TicketProject.Models.Entity;

namespace TicketProject.DAL.Interfaces
{
    public interface IUserDao
    {
        User GetUserById(int id);

        User GetUserByEmail(string email);

        User? GetUserByAccount(LoginUserDto loginUserDto);

        void AddUser(User user);

        void UpdateUser(User user);

        void DeleteUser(int id);

        IEnumerable<User> GetAllUsers();
    }
}
