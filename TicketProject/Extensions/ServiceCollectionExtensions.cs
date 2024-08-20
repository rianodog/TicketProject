using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using TicketProject.DAL.Interfaces;
using TicketProject.DAL.Implement;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;

namespace TicketProject.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTicketServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<TicketDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TicketDb")));

            services.AddLogging(options =>
            {
                options.ClearProviders();
                options.SetMinimumLevel(LogLevel.Debug);
                options.AddNLog(configuration);
            });

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserDao, UserDao>();
        }
    }
}
