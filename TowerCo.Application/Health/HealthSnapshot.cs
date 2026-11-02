using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Health;

/// <summary>Portfolio data loaded once and grouped by site, so every site is scored from the same snapshot.</summary>
internal sealed class HealthSnapshot(
    IReadOnlyList<SensorReading> readings, IReadOnlyList<Inspection> inspections, IReadOnlyList<WorkOrder> workOrders, DateOnly today)
{
    private static readonly WorkOrderStatus[] Closed = [WorkOrderStatus.Completed, WorkOrderStatus.Cancelled];

    public HealthContext ContextFor(Site site) => new(site, LatestReadings(site.SiteCode), LastInspection(site.Id),
        OpenWorkOrders(site.Id), today);

    public IReadOnlyList<SensorReading> ReadingsFor(string siteCode) =>
        readings.Where(r => r.SiteCode == siteCode).OrderBy(r => r.RecordedAtUtc).ToList();

    public IReadOnlyList<Inspection> InspectionsFor(int siteId) =>
        inspections.Where(i => i.SiteId == siteId).OrderByDescending(i => i.InspectedOn).ToList();

    public IReadOnlyList<WorkOrder> OpenWorkOrders(int siteId) =>
        workOrders.Where(w => w.SiteId == siteId && !Closed.Contains(w.Status)).ToList();

    private IReadOnlyList<SensorReading> LatestReadings(string siteCode) =>
        ReadingsFor(siteCode).GroupBy(r => r.SensorType).Select(g => g.Last()).ToList();

    private Inspection? LastInspection(int siteId) => InspectionsFor(siteId).FirstOrDefault();
}
