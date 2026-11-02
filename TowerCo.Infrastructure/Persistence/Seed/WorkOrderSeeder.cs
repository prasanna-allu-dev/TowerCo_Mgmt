using Microsoft.EntityFrameworkCore;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;

namespace TowerCo.Infrastructure.Persistence.Seed;

/// <summary>Adds fictional demo work orders across the portfolio. No-op when any work order exists.</summary>
internal static class WorkOrderSeeder
{
    private static readonly DateTime CreatedAtUtc = new(2026, 10, 20, 8, 0, 0, DateTimeKind.Utc);
    private static readonly string[] Engineers = ["Arjun Mehta", "Thandi Nkosi", "Wanjiru Kamau", "Chidi Okafor"];
    private static readonly (string Title, WorkOrderCategory Category, WorkOrderPriority Priority, WorkOrderStatus Target)[] Templates =
    [
        ("Quarterly preventive maintenance", WorkOrderCategory.Preventive, WorkOrderPriority.Medium, WorkOrderStatus.Assigned),
        ("Aviation obstruction light fault", WorkOrderCategory.Corrective, WorkOrderPriority.Critical, WorkOrderStatus.InProgress),
        ("Install tenant 5G antenna", WorkOrderCategory.Installation, WorkOrderPriority.High, WorkOrderStatus.Open),
        ("Annual structural inspection", WorkOrderCategory.Inspection, WorkOrderPriority.Low, WorkOrderStatus.Completed),
        ("Rectifier alarm investigation", WorkOrderCategory.Corrective, WorkOrderPriority.High, WorkOrderStatus.OnHold)
    ];

    public static async Task SeedAsync(TowerCoDbContext context)
    {
        if (await context.WorkOrders.AnyAsync())
        {
            return;
        }
        var sites = await context.Sites.OrderBy(s => s.Id).ToListAsync();
        context.WorkOrders.AddRange(sites.Select(Create));
        await context.SaveChangesAsync();
    }

    private static WorkOrder Create(Site site, int index)
    {
        var template = Templates[index % Templates.Length];
        var details = new WorkOrderDetails(template.Title, $"{template.Title} at {site.Name}.",
            template.Category, template.Priority, DateOnly.FromDateTime(CreatedAtUtc).AddDays(7 + 3 * index));
        var order = new WorkOrder(site, details, null, CreatedAtUtc.AddHours(index));
        MoveTo(order, template.Target, Engineers[index % Engineers.Length]);
        return order;
    }

    private static void MoveTo(WorkOrder order, WorkOrderStatus target, string engineer)
    {
        WorkOrderStatus[] path = target switch
        {
            WorkOrderStatus.Open => [],
            WorkOrderStatus.Assigned => [WorkOrderStatus.Assigned],
            WorkOrderStatus.InProgress => [WorkOrderStatus.Assigned, WorkOrderStatus.InProgress],
            WorkOrderStatus.Cancelled => [WorkOrderStatus.Cancelled],
            _ => [WorkOrderStatus.Assigned, WorkOrderStatus.InProgress, target]
        };
        foreach (var step in path)
        {
            order.ChangeStatus(step, engineer, CreatedAtUtc.AddDays(2));
        }
    }
}
