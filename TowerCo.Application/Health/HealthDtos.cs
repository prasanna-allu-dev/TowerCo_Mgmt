using TowerCo.Application.WorkOrders;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Health;

public sealed record HealthSignalDto(string Name, HealthStatus Status, string Reason);

public sealed record SiteHealthSummaryDto(
    int SiteId, string SiteCode, string SiteName, string CountryCode, SiteStatus SiteStatus,
    HealthStatus Overall, string MainReason, IReadOnlyList<HealthSignalDto> Signals);

/// <summary>Counts cover every site matching the country filter; the list also honours the status filter.</summary>
public sealed record PortfolioHealthDto(int Healthy, int Warning, int Critical, IReadOnlyList<SiteHealthSummaryDto> Sites);

public sealed record ReadingPointDto(DateTime RecordedAtUtc, decimal Value);

public sealed record SensorSeriesDto(
    SensorType SensorType, decimal Latest, HealthStatus Status, decimal Warning, decimal Critical,
    ThresholdDirection Direction, IReadOnlyList<ReadingPointDto> Points);

public sealed record InspectionDto(int Id, DateOnly InspectedOn, string Inspector, InspectionOutcome Outcome, string? Notes);

public sealed record SiteHealthDto(
    SiteHealthSummaryDto Summary, IReadOnlyList<SensorSeriesDto> Telemetry,
    IReadOnlyList<InspectionDto> Inspections, IReadOnlyList<WorkOrderDto> OpenWorkOrders);

public sealed class HealthFilter
{
    public string? Country { get; init; }
    public HealthStatus? Status { get; init; }
}
