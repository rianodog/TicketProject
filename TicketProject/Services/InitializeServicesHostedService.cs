using TicketProject.Factory.Interfaces;
using TicketProject.Services.Interfaces;

namespace TicketProject.Services
{
    public class InitializeServicesHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public InitializeServicesHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
