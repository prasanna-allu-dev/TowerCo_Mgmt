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

    /// <summary>
    /// Leases a slot to an operator for [startDate, endDate]. Enforces active site, no overlapping lease for the same
    /// operator, and a free slot on every day of the period (future-dated leases count).
    /// </summary>
    public Tenancy AddTenancy(Operator mobileOperator, DateOnly startDate, DateOnly? endDate, Money monthlyRent)
    {
        EnsureCanLease(mobileOperator, startDate, endDate);
        var tenancy = new Tenancy(this, mobileOperator, startDate, endDate, monthlyRent);
        _tenancies.Add(tenancy);
        return tenancy;
    }

    /// <summary>Gives notice: the tenancy ends on the given date and frees its slot after that.</summary>
    public void EndTenancy(Tenancy tenancy, DateOnly endDate) => Own(tenancy).End(endDate);

    /// <summary>Extends a fixed-term tenancy (null makes it open ended). The extra period must still fit capacity.</summary>
    public void RenewTenancy(Tenancy tenancy, DateOnly? newEndDate)
    {
        var currentEnd = Own(tenancy).EndDate;
        if (currentEnd is not null && PeakOccupancy(currentEnd.Value.AddDays(1), newEndDate, tenancy) >= MaxTenants)
        {
            throw new TowerCapacityExceededException(Site.SiteCode, MaxTenants);
        }
        tenancy.ExtendTo(newEndDate);
    }

    /// <summary>Applies a rent review in the tenancy's existing currency.</summary>
    public void ReviewRent(Tenancy tenancy, Money newMonthlyRent) => Own(tenancy).ReviewRent(newMonthlyRent);

    /// <summary>Changes slot capacity (structural upgrade or downgrade). Cannot drop below tenancies committed from asOf.</summary>
    public void ChangeCapacity(int maxTenants, DateOnly asOf)
    {
        Guard.AgainstNonPositive(maxTenants, nameof(maxTenants));
        var committed = PeakOccupancy(asOf, null, null);
        if (maxTenants < committed)
        {
            throw new CapacityBelowCommitmentsException(Site.SiteCode, maxTenants, committed);
        }
        MaxTenants = maxTenants;
    }

    /// <summary>Monthly rent of tenancies active on the date, per currency.</summary>
    public IReadOnlyList<Money> MonthlyRevenue(DateOnly asOf) => _tenancies.Where(t => t.IsActiveOn(asOf))
        .GroupBy(t => t.MonthlyRent.Currency)
        .Select(g => g.Select(t => t.MonthlyRent).Aggregate((total, rent) => total.Add(rent))).ToList();

    /// <summary>Most slots in use on any day of [start, end], ignoring one tenancy (for renewals).</summary>
    public int PeakOccupancy(DateOnly start, DateOnly? end, Tenancy? excluding)
    {
        var others = _tenancies.Where(t => !ReferenceEquals(t, excluding) && t.Overlaps(start, end)).ToList();
        var checkpoints = others.Select(t => t.StartDate).Where(d => d > start).Append(start);
        return checkpoints.Select(day => others.Count(t => t.IsActiveOn(day))).DefaultIfEmpty(0).Max();
    }

    /// <summary>Mounts equipment for a tenant. Every item is owned by a tenancy on this tower.</summary>
    public Equipment InstallEquipment(EquipmentType type, string serialNumber, DateOnly installedOn, decimal? mountHeightMetres, Tenancy tenancy)
    {
        if (!_tenancies.Contains(tenancy))
        {
            throw new InvalidEquipmentOwnerException(Site.SiteCode);
        }

        var equipment = new Equipment(this, type, serialNumber, installedOn, mountHeightMetres, tenancy);
        _equipment.Add(equipment);
        return equipment;
    }

    private void EnsureCanLease(Operator mobileOperator, DateOnly start, DateOnly? end)
    {
        if (Site.Status != SiteStatus.Active)
        {
            throw new SiteNotActiveException(Site.SiteCode, Site.Status);
        }

        if (_tenancies.Any(t => t.Overlaps(start, end) && t.IsHeldBy(mobileOperator)))
        {
            throw new DuplicateTenancyException(mobileOperator.Name, Site.SiteCode);
        }

        if (PeakOccupancy(start, end, null) >= MaxTenants)
        {
            throw new TowerCapacityExceededException(Site.SiteCode, MaxTenants);
        }
    }

    private Tenancy Own(Tenancy tenancy) => _tenancies.Contains(tenancy)
        ? tenancy
        : throw new NotFoundException(nameof(Tenancy), tenancy.Id);
}
