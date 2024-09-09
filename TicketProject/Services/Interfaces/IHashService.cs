namespace TicketProject.Services.Interfaces
{
    public interface IHashService
    {
        Task<string> HashPassword(string password);
    }
}
