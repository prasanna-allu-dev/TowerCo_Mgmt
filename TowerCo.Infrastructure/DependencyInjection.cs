using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TowerCo.Application.Abstractions;
using TowerCo.Infrastructure.Exceptions;
using TowerCo.Infrastructure.Persistence.Repositories;
using TowerCo.Infrastructure.Persistence;
using TowerCo.Infrastructure.Persistence.Seed;

namespace TowerCo.Infrastructure;

/// <summary>Registers persistence and other infrastructure services.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(InfrastructureConstants.ConnectionStringName)
            ?? throw new MissingConfigurationException($"ConnectionStrings:{InfrastructureConstants.ConnectionStringName}");

        services.AddDbContext<TowerCoDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ITowerRepository, TowerRepository>();
        services.AddScoped<IOperatorRepository, OperatorRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IEquipmentTypeRepository, EquipmentTypeRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
        services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
        services.AddScoped<IInspectionRepository, InspectionRepository>();
        return services;
    }

    /// <summary>Creates the SQLite schema and seeds demo data on first run (demo setup, no migrations).</summary>
    public static async Task InitialiseDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TowerCoDbContext>();
        await context.Database.EnsureCreatedAsync();
        await DatabaseSeeder.SeedAsync(context);
        await WorkOrderSeeder.SeedAsync(context);
        await HealthSeeder.SeedAsync(context);
    }
}
