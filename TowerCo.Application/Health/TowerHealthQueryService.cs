using Microsoft.Extensions.Options;
using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Application.WorkOrders;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Health;

public interface ITowerHealthQueryService
{
    Task<PortfolioHealthDto> GetPortfolioAsync(HealthFilter filter, CancellationToken cancellationToken);
    Task<SiteHealthDto> GetSiteAsync(int siteId, CancellationToken cancellationToken);
}

/// <summary>UC5: tower health from telemetry, inspections and open work orders. Overall status is the worst signal.</summary>
internal sealed class TowerHealthQueryService(
    HealthDataSource data, IEnumerable<IHealthSignalRule> rules, IOptions<SensorThresholdOptions> thresholds)
    : ITowerHealthQueryService
{
    public async Task<PortfolioHealthDto> GetPortfolioAsync(HealthFilter filter, CancellationToken cancellationToken)
    {
        var (sites, snapshot) = await data.LoadAsync(null, cancellationToken);
        var scored = sites.Where(s => TextFilter.EqualsCode(filter.Country, s.Location.CountryCode))
            .Select(s => Summarise(s, snapshot)).ToList();
        var listed = scored.Where(s => filter.Status is null || s.Overall == filter.Status)
            .OrderByDescending(s => s.Overall).ThenBy(s => s.SiteCode).ToList();
        return new(Count(scored, HealthStatus.Healthy), Count(scored, HealthStatus.Warning), Count(scored, HealthStatus.Critical), listed);
    }

    public async Task<SiteHealthDto> GetSiteAsync(int siteId, CancellationToken cancellationToken)
    {
        var (sites, snapshot) = await data.LoadAsync(siteId, cancellationToken);
        var site = sites.FirstOrDefault(s => s.Id == siteId) ?? throw new NotFoundException(nameof(Site), siteId);
        return new(Summarise(site, snapshot), Telemetry(snapshot.ReadingsFor(site.SiteCode)),
            snapshot.InspectionsFor(site.Id).Select(i => new InspectionDto(i.Id, i.InspectedOn, i.Inspector, i.Outcome, i.Notes)).ToList(),
            snapshot.OpenWorkOrders(site.Id).Select(WorkOrderDto.From).ToList());
    }

    private SiteHealthSummaryDto Summarise(Site site, HealthSnapshot snapshot)
    {
        var context = snapshot.ContextFor(site);
        var signals = rules.Select(rule => rule.Evaluate(context)).ToList();
        var worst = signals.OrderByDescending(s => s.Status).First();
        return new(site.Id, site.SiteCode, site.Name, site.Location.CountryCode, site.Status, worst.Status, worst.Reason,
            signals.Select(s => new HealthSignalDto(s.Name, s.Status, s.Reason)).ToList());
    }

    private List<SensorSeriesDto> Telemetry(IReadOnlyList<SensorReading> readings) =>
        readings.GroupBy(r => r.SensorType).OrderBy(g => g.Key).Select(g => ToSeries(g.Key, g.ToList())).ToList();

    private SensorSeriesDto ToSeries(SensorType type, IReadOnlyList<SensorReading> points)
    {
        var limit = thresholds.Value[type];
        var latest = points[^1].Value;
        return new(type, latest, HealthSignal.FromAlarm(limit.Evaluate(latest)), limit.Warning, limit.Critical, limit.Direction,
            points.Select(p => new ReadingPointDto(p.RecordedAtUtc, p.Value)).ToList());
    }

    private static int Count(IEnumerable<SiteHealthSummaryDto> sites, HealthStatus status) => sites.Count(s => s.Overall == status);
}
