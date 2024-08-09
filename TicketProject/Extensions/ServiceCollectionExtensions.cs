using Microsoft.EntityFrameworkCore;

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


        }
    }
}
