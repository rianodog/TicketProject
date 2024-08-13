using TicketProject.Models.DTO;
using TicketProject.Models.Entity;

namespace TicketProject.Services.Interfaces
{
    public interface IUserService
    {
        User GetUserById(int id);

        User GetUserByEmail(string email);

        void RegisterUser(User user);

        User? LoginUser(LoginUserDto loginUserDto);

        void UpdateUser(User user);

        void DeleteUser(int id);

        IEnumerable<User> GetAllUsers();
    }
}
