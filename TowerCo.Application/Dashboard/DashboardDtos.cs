using System.Text.Json;
using System.Text.Json.Serialization;

namespace TowerCo.Application.Dashboard;

public sealed record PortfolioKpisDto(
    int TotalSites, int ActiveSites, int TotalSlots, int OccupiedSlots, decimal UtilisationPercent, int FullTowers, int EmptyTowers);

public sealed record CountryUtilisationDto(string CountryCode, int SiteCount, int OccupiedSlots, int TotalSlots, decimal UtilisationPercent);

public sealed record CurrencyAmountDto(string Currency, decimal Amount);

public sealed record OperatorShareDto(int OperatorId, string OperatorName, int ActiveTenancies);

public sealed record ExpiringTenancyDto(int TenancyId, int SiteId, string SiteCode, string OperatorName, DateOnly EndDate, int DaysRemaining);

public sealed record SellableTowerDto(int SiteId, string SiteCode, string SiteName, string CountryCode, int AvailableSlots);

/// <summary>Declared in priority order: alerts are sorted most severe first.</summary>
[JsonConverter(typeof(AlertSeverityJsonConverter))]
public enum AlertSeverity
{
    Critical,
    Warning,
    Neutral,
    Ok
}

/// <summary>Serialises severity in lower camel case ("critical") to match the client contract.</summary>
public sealed class AlertSeverityJsonConverter() : JsonStringEnumConverter<AlertSeverity>(JsonNamingPolicy.CamelCase);

public sealed record DashboardAlertDto(string Id, AlertSeverity Severity, string Title, string Detail, int? SiteId);

public sealed record PortfolioDashboardDto(
    PortfolioKpisDto Kpis,
    IReadOnlyDictionary<string, int> SitesByStatus,
    IReadOnlyList<CountryUtilisationDto> UtilisationByCountry,
    IReadOnlyList<CurrencyAmountDto> MonthlyRentByCurrency,
    IReadOnlyList<OperatorShareDto> OperatorShares,
    IReadOnlyList<ExpiringTenancyDto> ExpiringTenancies,
    IReadOnlyList<SellableTowerDto> SellableTowers,
    IReadOnlyList<DashboardAlertDto> Alerts);
