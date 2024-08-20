using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly ILogger<UserService> _logger;
        private readonly IErrorHandler _errorHandler;

        public UserService(IUserDao userDao, ILogger<UserService> logger, IErrorHandler errorHandler)
        {
            _userDao = userDao;
            _logger = logger;
            _errorHandler = errorHandler;
        }

        public User? GetUserByEmail(string email)
        {
            try
            {
                return _userDao.GetUser(u => u.Email == email).Result;
            }
            catch(Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        public User? GetUserByPhoneNumber(string phoneNumber)
        {
            try
            {
                return _userDao.GetUser(u => u.PhoneNumber == phoneNumber!).Result;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        public async Task<User?> GetUserByLogin(LoginUserDto loginUserDto)
        {
            try
            {
                var paswordHash = await HashPassword(loginUserDto.Password!);
                return await _userDao.GetUser(u => (u.Email == loginUserDto.Account || u.PhoneNumber == loginUserDto.Account) 
                    && u.PasswordHash == paswordHash);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GeUsersByFilter(Expression<Func<User, bool>> filter)
        {
            try
            {
                return await _userDao.GetUsers(filter);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                return await _userDao.GetAllUsers();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        public async Task<User> RegisterUser(User user)
        {
            try
            {
                user.PasswordHash = await HashPassword(user.PasswordHash!);
                return await _userDao.AddUser(user);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        public async Task<User> UpdateUser(User user)
        {
            try
            {
                return await _userDao.UpdateUser(user);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        public async Task DeleteUser(int id)
        {
            try
            {
                var user = await _userDao.GetUser(u => u.UserId == id);
                if (user != null)
                {
                    await _userDao.DeleteUser(user);
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }

        private async Task<string> HashPassword(string password)
        {
            try
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(password));
                var bytes = await SHA256.HashDataAsync(stream);
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e, _logger, typeof(UserService).FullName!);
                throw;
            }
        }
    }
}

