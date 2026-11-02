using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Application.Constants;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Tenants;

public sealed record TenantRecordDto(
    int TenancyId, int OperatorId, string OperatorName, int SiteId, string SiteCode, string SiteName,
    string CountryCode, int TowerId, DateOnly StartDate, DateOnly? EndDate, decimal MonthlyRent,
    string Currency, bool IsActive, int EquipmentCount);

/// <summary>Filters for the tenants register. State is "active" or "ended".</summary>
public sealed class TenantFilter
{
    [StringLength(ValidationRules.SearchMaxLength)]
    public string? Search { get; init; }
    [Range(ValidationRules.MinId, int.MaxValue)]
    public int? OperatorId { get; init; }
    [RegularExpression(ValidationRules.TenancyStatePattern)]
    public string? State { get; init; }
    [RegularExpression(ValidationRules.CountryCodePattern)]
    public string? Country { get; init; }
}

public interface ITenantQueryService
{
    Task<IReadOnlyList<TenantRecordDto>> ListAsync(TenantFilter filter, CancellationToken cancellationToken);
}

/// <summary>Portfolio-wide tenancy register (one row per tenancy).</summary>
internal sealed class TenantQueryService(ITowerRepository towers, TimeProvider timeProvider) : ITenantQueryService
{
    public async Task<IReadOnlyList<TenantRecordDto>> ListAsync(TenantFilter filter, CancellationToken cancellationToken)
    {
        var today = timeProvider.Today();
        var all = await towers.ListAsync(cancellationToken);
        return all.SelectMany(tower => tower.Tenancies.Select(t => ToRecord(tower, t, today)))
            .Where(record => Matches(record, filter))
            .OrderBy(record => record.SiteCode).ThenBy(record => record.OperatorName).ToList();
    }

    private static TenantRecordDto ToRecord(Tower tower, Tenancy tenancy, DateOnly today) => new(
        tenancy.Id, tenancy.OperatorId, tenancy.Operator.Name, tower.SiteId, tower.Site.SiteCode, tower.Site.Name,
        tower.Site.Location.CountryCode, tower.Id, tenancy.StartDate, tenancy.EndDate,
        tenancy.MonthlyRent.Amount, tenancy.MonthlyRent.Currency, tenancy.IsActiveOn(today),
        tower.Equipment.Count(e => e.TenancyId == tenancy.Id && e.Status != EquipmentStatus.Removed));

    private static bool Matches(TenantRecordDto record, TenantFilter filter) =>
        TextFilter.Matches(filter.Search, record.OperatorName, record.SiteCode, record.SiteName)
        && (filter.OperatorId is null || record.OperatorId == filter.OperatorId)
        && (string.IsNullOrWhiteSpace(filter.State) || (filter.State == FilterValues.TenancyActive) == record.IsActive)
        && TextFilter.EqualsCode(filter.Country, record.CountryCode);
}
