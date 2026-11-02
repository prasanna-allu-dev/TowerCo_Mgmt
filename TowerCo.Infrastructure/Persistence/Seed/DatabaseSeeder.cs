using Microsoft.EntityFrameworkCore;

namespace TowerCo.Infrastructure.Persistence.Seed;

/// <summary>Loads the demo portfolio into an empty database. No-op when sites already exist.</summary>
internal static class DatabaseSeeder
{
    public static async Task SeedAsync(TowerCoDbContext context)
    {
        if (await context.Sites.AnyAsync())
        {
            return;
        }

        var data = SeedData.Create();
        context.AddRange(data.Operators);
        context.AddRange(data.EquipmentTypes);
        context.AddRange(data.Sites);
        await context.SaveChangesAsync();
    }
}
