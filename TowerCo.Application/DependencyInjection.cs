using Microsoft.Extensions.DependencyInjection;
using TowerCo.Application.Dashboard;
using TowerCo.Application.EquipmentRegister;
using TowerCo.Application.Health;
using TowerCo.Application.Tenants;
using TowerCo.Application.WorkOrders;
using TowerCo.Application.Operators;
using TowerCo.Application.Sites;
using TowerCo.Application.Tenancies;
using TowerCo.Application.Towers;

namespace TowerCo.Application;

/// <summary>Registers application-layer use-case services.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ISiteQueryService, SiteQueryService>();
        services.AddScoped<ITowerQueryService, TowerQueryService>();
        services.AddScoped<IOperatorQueryService, OperatorQueryService>();
        services.AddScoped<ITenancyService, TenancyService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISiteCommandService, SiteCommandService>();
        services.AddScoped<ITowerCommandService, TowerCommandService>();
        services.AddScoped<ITenantQueryService, TenantQueryService>();
        services.AddScoped<IEquipmentQueryService, EquipmentQueryService>();
        services.AddScoped<IWorkOrderService, WorkOrderService>();
        return services.AddHealth();
    }

    private static IServiceCollection AddHealth(this IServiceCollection services)
    {
        services.AddScoped<HealthDataSource>();
        services.AddScoped<ITowerHealthQueryService, TowerHealthQueryService>();
        services.AddSingleton<IHealthSignalRule, TelemetryHealthRule>();
        services.AddSingleton<IHealthSignalRule, InspectionHealthRule>();
        services.AddSingleton<IHealthSignalRule, WorkOrderHealthRule>();
        return services;
    }
}
