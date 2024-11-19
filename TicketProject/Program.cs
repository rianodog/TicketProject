namespace TicketProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = 5000; // �]�w���һݪ��ƶq
                options.Limits.MaxRequestBodySize = null; // �L����γ]�m�A��j�p
            });

            ThreadPool.SetMaxThreads(500, 500); // �]�w���һݪ��ƶq

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
