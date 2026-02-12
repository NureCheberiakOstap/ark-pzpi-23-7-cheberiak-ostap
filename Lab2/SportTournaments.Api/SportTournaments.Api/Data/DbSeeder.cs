using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Entities;

namespace SportTournaments.Api.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAsync(ApplicationDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Roles.AnyAsync()) return;

        db.Roles.AddRange(
            new Role { Name = "Admin" },
            new Role { Name = "Organizer" },
            new Role { Name = "Judge" },
            new Role { Name = "Participant" },
            new Role { Name = "Viewer" }
        );

        await db.SaveChangesAsync();
    }
}
