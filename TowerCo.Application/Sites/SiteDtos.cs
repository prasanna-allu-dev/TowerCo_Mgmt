using TowerCo.Domain.Enums;

namespace TowerCo.Application.Sites;

public sealed record CapacitySummaryDto(int MaxTenants, int ActiveTenantCount, int AvailableSlots, decimal UtilisationPercent, bool HasCapacity);

public sealed record SiteSummaryDto(
    int Id, string SiteCode, string Name, string CountryCode, string City, SiteStatus Status, int TowerId, CapacitySummaryDto Capacity);

public sealed record TowerDto(int Id, TowerType TowerType, decimal HeightMetres, int MaxTenants);

public sealed record TenancyDto(
    int Id, int OperatorId, string OperatorName, DateOnly StartDate, DateOnly? EndDate, decimal MonthlyRent, string Currency);

public sealed record EquipmentDto(
    int Id, string TypeName, EquipmentCategory Category, int TenancyId, string Owner,
    string SerialNumber, DateOnly InstalledOn, decimal? MountHeightMetres, EquipmentStatus Status);

/// <summary>Everything about a site: summary fields plus location, tower, tenancies and equipment.</summary>
public sealed record SiteDetailDto(
    int Id, string SiteCode, string Name, string CountryCode, string City, SiteStatus Status, int TowerId,
    CapacitySummaryDto Capacity, double Latitude, double Longitude, TowerDto Tower,
    IReadOnlyList<TenancyDto> Tenancies, IReadOnlyList<EquipmentDto> Equipment);
