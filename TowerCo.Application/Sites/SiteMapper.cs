using TowerCo.Domain.Entities;

namespace TowerCo.Application.Sites;

/// <summary>Maps the tower aggregate (with its site) to API DTOs. Capacity is derived as of the given date.</summary>
public static class SiteMapper
{
    public static CapacitySummaryDto ToCapacity(Tower tower, DateOnly asOf) => new(
        tower.MaxTenants, tower.ActiveTenantCount(asOf), tower.AvailableSlots(asOf),
        tower.UtilisationPercent(asOf), tower.HasCapacity(asOf));

    public static SiteSummaryDto ToSummary(Tower tower, DateOnly asOf)
    {
        var site = tower.Site;
        return new(site.Id, site.SiteCode, site.Name, site.Location.CountryCode, site.Location.City,
            site.Status, tower.Id, ToCapacity(tower, asOf));
    }

    public static SiteDetailDto ToDetail(Tower tower, DateOnly asOf)
    {
        var site = tower.Site;
        return new(site.Id, site.SiteCode, site.Name, site.Location.CountryCode, site.Location.City, site.Status,
            tower.Id, ToCapacity(tower, asOf), site.Location.Latitude, site.Location.Longitude,
            new TowerDto(tower.Id, tower.TowerType, tower.HeightMetres, tower.MaxTenants),
            tower.Tenancies.OrderBy(t => t.StartDate).Select(ToTenancy).ToList(),
            tower.Equipment.Select(ToEquipment).ToList(),
            tower.Infrastructure.Select(i => new SiteInfrastructureDto(i.Id, i.Kind, i.SerialNumber, i.InstalledOn)).ToList());
    }

    public static TenancyDto ToTenancy(Tenancy tenancy) => new(
        tenancy.Id, tenancy.OperatorId, tenancy.Operator.Name, tenancy.StartDate, tenancy.EndDate,
        tenancy.MonthlyRent.Amount, tenancy.MonthlyRent.Currency);

    private static EquipmentDto ToEquipment(Equipment equipment) => new(
        equipment.Id, equipment.EquipmentType.Name, equipment.EquipmentType.Category,
        equipment.TenancyId, equipment.Tenancy.Operator.Name,
        equipment.SerialNumber, equipment.InstalledOn, equipment.MountHeightMetres, equipment.Status);
}
