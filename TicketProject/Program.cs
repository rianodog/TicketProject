namespace TicketProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = 5000; // 設定為所需的數量
                options.Limits.MaxRequestBodySize = null; // 無限制或設置適當大小
            });

            ThreadPool.SetMaxThreads(500, 500); // 設定為所需的數量

            // Add services to the container.
            Services.ServiceCollectionExtensions.AddTicketServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
