using Leakzon.WebApi.Extension;
using Leakzone.Backend.Accessors.SensorAccessor;
using Leakzone.Backend.Configuration;
using Leakzone.Backend.Infrastructure;
using Leakzone.Backend.Managers;

namespace Leakzon.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var services = builder.Services;

            // Add services to the container.

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper();
            services.AddConfiguration<DbConfiguration>(configuration, "DbConfiguration");
            services.AddSingleton<IDbContext, MongoDbContext>();
            services.AddSingleton<ISensorReadingManager, SensorReadingManager>();
            services.AddSingleton<ISensorAccessor, SensorAccessor>();
           
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
