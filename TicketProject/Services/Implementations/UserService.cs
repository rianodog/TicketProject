using TicketProject.DAL.Interfaces;
using TicketProject.Models.DTO;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;

        public UserService(IUserDao userDao)
        {
            _userDao = userDao;
        }


        public User GetUserById(int id)
        {
            return _userDao.GetUserById(id);
        }

        public User GetUserByEmail(string email)
        {
            return _userDao.GetUserByEmail(email);
        }

        public void RegisterUser(User user)
        {
            _userDao.AddUser(user);
        }

        public User? LoginUser(LoginUserDto loginUserDto)
        {
            var user = _userDao.GetUserByAccount(loginUserDto);
            if (user != null && user.PasswordHash == loginUserDto.Password) // 假設密碼是明文存儲，實際應用中應該使用哈希
            {
                return user;
            }
            return null;
        }

        public void UpdateUser(User user)
        {
            _userDao.UpdateUser(user);
        }

        public void DeleteUser(int id)
        {
            _userDao.DeleteUser(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userDao.GetAllUsers();
        }
    }
}

