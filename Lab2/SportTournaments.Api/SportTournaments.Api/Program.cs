using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Data;

namespace SportTournaments.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                DbSeeder.SeedRolesAsync(db).GetAwaiter().GetResult();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();
            app.Run();

        }
    }
}
