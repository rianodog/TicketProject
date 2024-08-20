namespace TicketProject.Services.Interfaces
{
    public interface IErrorHandler
    {
        void HandleError(Exception e, ILogger logger, string source);
    }
}
