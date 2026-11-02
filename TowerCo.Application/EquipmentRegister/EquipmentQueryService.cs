using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Application.Constants;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.EquipmentRegister;

public sealed record EquipmentItemDto(
    int Id, int TowerId, int SiteId, string SiteCode, string SiteName, string CountryCode,
    int TypeId, string TypeName, EquipmentCategory Category, int? TenancyId, string? OwnerName,
    string SerialNumber, DateOnly InstalledOn, decimal? MountHeightMetres, EquipmentStatus Status);

public sealed record EquipmentTypeSummaryDto(int Id, string Name, EquipmentCategory Category, int ItemCount);

/// <summary>Filters for the equipment register. Owner is "tenant" or "shared".</summary>
public sealed class EquipmentFilter
{
    [StringLength(ValidationRules.SearchMaxLength)]
    public string? Search { get; init; }
    public EquipmentCategory? Category { get; init; }
    [RegularExpression(ValidationRules.OwnerPattern)]
    public string? Owner { get; init; }
    public EquipmentStatus? Status { get; init; }
    [RegularExpression(ValidationRules.CountryCodePattern)]
    public string? Country { get; init; }
    [Range(ValidationRules.MinId, int.MaxValue)]
    public int? SiteId { get; init; }
}

public interface IEquipmentQueryService
{
    Task<IReadOnlyList<EquipmentItemDto>> ListAsync(EquipmentFilter filter, CancellationToken cancellationToken);
    Task<IReadOnlyList<EquipmentTypeSummaryDto>> ListTypesAsync(CancellationToken cancellationToken);
}

internal sealed class EquipmentQueryService(ITowerRepository towers, IEquipmentTypeRepository types) : IEquipmentQueryService
{
    public async Task<IReadOnlyList<EquipmentItemDto>> ListAsync(EquipmentFilter filter, CancellationToken cancellationToken)
    {
        var all = await towers.ListAsync(cancellationToken);
        return all.SelectMany(tower => tower.Equipment.Select(e => ToItem(tower, e)))
            .Where(item => Matches(item, filter))
            .OrderBy(item => item.SiteCode).ThenByDescending(item => item.MountHeightMetres).ToList();
    }

    public async Task<IReadOnlyList<EquipmentTypeSummaryDto>> ListTypesAsync(CancellationToken cancellationToken)
    {
        var installed = (await towers.ListAsync(cancellationToken)).SelectMany(t => t.Equipment)
            .Where(e => e.Status != EquipmentStatus.Removed).ToList();
        var all = await types.ListAsync(cancellationToken);
        return all.Select(t => new EquipmentTypeSummaryDto(t.Id, t.Name, t.Category,
            installed.Count(e => e.EquipmentTypeId == t.Id))).ToList();
    }

    private static EquipmentItemDto ToItem(Tower tower, Equipment equipment) => new(
        equipment.Id, tower.Id, tower.SiteId, tower.Site.SiteCode, tower.Site.Name, tower.Site.Location.CountryCode,
        equipment.EquipmentTypeId, equipment.EquipmentType.Name, equipment.EquipmentType.Category,
        equipment.TenancyId, equipment.Tenancy?.Operator.Name, equipment.SerialNumber,
        equipment.InstalledOn, equipment.MountHeightMetres, equipment.Status);

    private static bool Matches(EquipmentItemDto item, EquipmentFilter filter) =>
        TextFilter.Matches(filter.Search, item.SerialNumber, item.TypeName, item.SiteCode, item.SiteName, item.OwnerName ?? DisplayLabels.SharedEquipmentOwner)
        && (string.IsNullOrWhiteSpace(filter.Owner) || (filter.Owner == FilterValues.OwnerTenant) == (item.TenancyId is not null))
        && (filter.Category is null || item.Category == filter.Category)
        && (filter.Status is null || item.Status == filter.Status)
        && TextFilter.EqualsCode(filter.Country, item.CountryCode)
        && (filter.SiteId is null || item.SiteId == filter.SiteId);
}
