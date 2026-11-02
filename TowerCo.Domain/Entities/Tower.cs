using TowerCo.Domain.Common;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Domain.Entities;

/// <summary>Structure on a site. Capacity is slot based: MaxTenants operators at a time.</summary>
public sealed class Tower : Entity
{
    private readonly List<Tenancy> _tenancies = [];
    private readonly List<Equipment> _equipment = [];

    private Tower() { }

    internal Tower(Site site, TowerType towerType, decimal heightMetres, int maxTenants)
    {
        Guard.AgainstNonPositive(heightMetres, nameof(heightMetres));
        Guard.AgainstNonPositive(maxTenants, nameof(maxTenants));
        Site = site;
        TowerType = towerType;
        HeightMetres = heightMetres;
        MaxTenants = maxTenants;
    }

    public int SiteId { get; private set; }
    public Site Site { get; private set; } = null!;
    public TowerType TowerType { get; private set; }
    public decimal HeightMetres { get; private set; }
    public int MaxTenants { get; private set; }
    public IReadOnlyCollection<Tenancy> Tenancies => _tenancies;
    public IReadOnlyCollection<Equipment> Equipment => _equipment;

    public int ActiveTenantCount(DateOnly asOf) => _tenancies.Count(t => t.IsActiveOn(asOf));

    public int AvailableSlots(DateOnly asOf) => Math.Max(0, MaxTenants - ActiveTenantCount(asOf));

    public bool HasCapacity(DateOnly asOf) => AvailableSlots(asOf) > 0;

    public decimal UtilisationPercent(DateOnly asOf) => Math.Round(
        ActiveTenantCount(asOf) * DomainConstants.PercentMultiplier / (decimal)MaxTenants,
        DomainConstants.UtilisationDecimals);

    /// <summary>Leases a slot to an operator. Enforces active site, no duplicate operator and free capacity on the start date.</summary>
    public Tenancy AddTenancy(Operator mobileOperator, DateOnly startDate, DateOnly? endDate, Money monthlyRent)
    {
        EnsureCanLease(mobileOperator, startDate);
        var tenancy = new Tenancy(this, mobileOperator, startDate, endDate, monthlyRent);
        _tenancies.Add(tenancy);
        return tenancy;
    }

    /// <summary>Mounts equipment. A null tenancy means TowerCo shared infrastructure.</summary>
    public Equipment InstallEquipment(EquipmentType type, string serialNumber, DateOnly installedOn, decimal? mountHeightMetres, Tenancy? tenancy)
    {
        var equipment = new Equipment(this, type, serialNumber, installedOn, mountHeightMetres, tenancy);
        _equipment.Add(equipment);
        return equipment;
    }

    private void EnsureCanLease(Operator mobileOperator, DateOnly asOf)
    {
        if (Site.Status != SiteStatus.Active)
        {
            throw new SiteNotActiveException(Site.SiteCode, Site.Status);
        }

        if (_tenancies.Any(t => t.IsActiveOn(asOf) && t.IsHeldBy(mobileOperator)))
        {
            throw new DuplicateTenancyException(mobileOperator.Name, Site.SiteCode);
        }

        if (!HasCapacity(asOf))
        {
            throw new TowerCapacityExceededException(Site.SiteCode, MaxTenants);
        }
    }
}
