using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using TicketProject.DAL.Interfaces;
using TicketProject.DAL.Implement;
using TicketProject.Services.Implement;
using TicketProject.Services.Interfaces;
using TicketProject.DAL;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using TicketProject.Factory.Interfaces;
using TicketProject.Factory.Implement;

namespace TicketProject.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTicketServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<WriteTicketDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("WriteTicketDb")));
            services.AddDbContext<ReadTicketDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ReadTicketDb")));

            services.AddLogging(options =>
            {
                options.ClearProviders();
                options.SetMinimumLevel(LogLevel.Debug);
                options.AddNLog(configuration);
            });

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                    };
                });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Singleton
            services.AddSingleton<IRedisServiceFactory, RedisServiceFactory>();
            services.AddSingleton<IRabbitMQServiceFactory, RabbitMQServiceFactory>();
            services.AddSingleton(serviesProvider => serviesProvider.GetRequiredService<IRabbitMQServiceFactory>().Create());
            services.AddSingleton(serviesProvider => serviesProvider.GetRequiredService<IRedisServiceFactory>().Create());
            services.AddSingleton<ITicketService, TicketService>();

            // Force to initialize services
            services.AddHostedService<InitializeServicesHostedService>();

            // 泛型服務註冊方式
            services.AddTransient(typeof(IErrorHandler<>), typeof(ErrorHandler<>));
            services.AddTransient(typeof(IDynamicQueryBuilderService<>), typeof(DynamicQueryBuilderService<>));

            services.AddTransient<IUserWriteDao, UserWriteDao>();
            services.AddTransient<IUserReadDao, UserReadDao>();
            services.AddTransient<ICampaignWriteDao, CampaignWriteDao>();
            services.AddTransient<ICampaignReadDao, CampaignReadDao>();
            services.AddTransient<ITicketContentWriteDao, TicketContentWriteDao>();
            services.AddTransient<ITicketContentReadDao, TicketContentReadDao>();
            services.AddTransient<IOrderWriteDao, OrderWriteDao>();
            services.AddTransient<ITicketWriteDao, TicketWriteDao>();


            services.AddTransient<IJWTService, JWTService>();
            services.AddTransient<IHashService, HashService>();
            services.AddTransient<IRetryService, RetryService>();


        }
    }
}
