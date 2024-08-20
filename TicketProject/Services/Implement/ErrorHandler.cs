using Microsoft.AspNetCore.Diagnostics;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    public class ErrorHandler : IErrorHandler
    {
        public void HandleError(Exception e, ILogger logger, string source)
        {
            if (string.IsNullOrEmpty(e.Source))
            {
                e.Source = source;
                logger.LogError(e.ToString());
            }
        }
    }
}
