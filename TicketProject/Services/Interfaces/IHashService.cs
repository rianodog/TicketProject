namespace TicketProject.Services.Interfaces
{
    public interface IHashService
    {
        Task<string> BcryptHashPassword(string password);
        Task<bool> VerifyPasswordBcrypt(string password, string hashedPassword);
    }
}
