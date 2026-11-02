using Microsoft.Extensions.Options;
using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Domain.Entities;

namespace TowerCo.Application.Health;

/// <summary>Loads sites plus the telemetry window, inspections and work orders needed to score health.</summary>
internal sealed class HealthDataSource(
    ISiteRepository sites, ISensorReadingRepository readings, IInspectionRepository inspections,
    IWorkOrderRepository workOrders, TimeProvider timeProvider, IOptions<HealthOptions> options)
{
    public async Task<(IReadOnlyList<Site> Sites, HealthSnapshot Snapshot)> LoadAsync(int? siteId, CancellationToken cancellationToken)
    {
        var allSites = await sites.ListAsync(cancellationToken);
        var siteCode = allSites.FirstOrDefault(s => s.Id == siteId)?.SiteCode;
        var since = timeProvider.GetUtcNow().UtcDateTime.AddDays(-options.Value.TelemetryWindowDays);
        var snapshot = new HealthSnapshot(
            await readings.ListSinceAsync(since, siteCode, cancellationToken),
            await inspections.ListAsync(siteId, cancellationToken),
            await workOrders.ListAsync(cancellationToken),
            timeProvider.Today());
        return (allSites, snapshot);
    }
}
