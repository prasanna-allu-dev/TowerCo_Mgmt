using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;

namespace TowerCo.Infrastructure.Persistence.Seed;

/// <summary>Builds a fresh entity graph from SeedCatalogue through domain methods, so seed data obeys the same invariants as live data.</summary>
internal sealed class SeedData
{
    private const int RadioMountOffsetMetres = 2;
    private const string SerialFormat = "{0}-EQ-{1:D3}";
    private const string InfrastructureSerialFormat = "{0}-INF-{1:D3}";

    private readonly Dictionary<string, Operator> _operators;
    private readonly Dictionary<string, EquipmentType> _equipmentTypes;

    private SeedData()
    {
        _operators = SeedCatalogue.Operators.ToDictionary(o => o.Name, o => new Operator(o.Name, o.CountryCode));
        _equipmentTypes = SeedCatalogue.EquipmentTypes.ToDictionary(e => e.Name, e => new EquipmentType(e.Name, e.Category));
        Sites = SeedCatalogue.Sites.Select(BuildSite).ToList();
    }

    public IReadOnlyCollection<Operator> Operators => _operators.Values;
    public IReadOnlyCollection<EquipmentType> EquipmentTypes => _equipmentTypes.Values;
    public IReadOnlyList<Site> Sites { get; }

    public static SeedData Create() => new();

    private Site BuildSite(SiteSeed seed)
    {
        var site = new Site(seed.Code, seed.Name, seed.Location, seed.Status, SeedCatalogue.SeededAtUtc);
        var tower = site.AddTower(seed.Tower.Type, seed.Tower.HeightMetres, seed.Tower.MaxTenants);
        foreach (var tenancySeed in seed.Tenancies)
        {
            AddTenancyWithRadio(tower, tenancySeed);
        }

        if (site.Status == SiteStatus.Active)
        {
            InstallPowerKit(tower);
        }

        return site;
    }

    /// <summary>TowerCo power on every active tower: solar on rooftops, a diesel generator elsewhere, plus a battery bank.</summary>
    private static void InstallPowerKit(Tower tower)
    {
        var installedOn = DateOnly.FromDateTime(SeedCatalogue.SeededAtUtc);
        var primary = tower.TowerType == TowerType.Rooftop ? SiteInfrastructureKind.Solar : SiteInfrastructureKind.Generator;
        foreach (var kind in new[] { primary, SiteInfrastructureKind.Battery })
        {
            var serial = string.Format(InfrastructureSerialFormat, tower.Site.SiteCode, tower.Infrastructure.Count + 1);
            tower.InstallInfrastructure(kind, serial, installedOn);
        }
    }

    /// <summary>Each tenancy gets a radio plus a microwave backhaul dish; both are removed when the tenancy has ended.</summary>
    private void AddTenancyWithRadio(Tower tower, TenancySeed seed)
    {
        var rent = SeedCatalogue.MonthlyRentByCountry[tower.Site.Location.CountryCode];
        var tenancy = tower.AddTenancy(_operators[seed.OperatorName], seed.Start, seed.End, rent);
        var radioName = tower.Tenancies.Count % 2 == 1 ? SeedCatalogue.FiveGAntenna : SeedCatalogue.FourGRadio;
        var mountHeight = tower.HeightMetres - RadioMountOffsetMetres;
        Equipment[] items =
        [
            Install(tower, radioName, seed.Start, mountHeight, tenancy),
            Install(tower, SeedCatalogue.MicrowaveDish, seed.Start, mountHeight, tenancy)
        ];
        if (seed.End is not null)
        {
            Array.ForEach(items, item => item.Remove());
        }
    }

    private Equipment Install(Tower tower, string typeName, DateOnly installedOn, decimal? mountHeight, Tenancy tenancy)
    {
        var serial = string.Format(SerialFormat, tower.Site.SiteCode, tower.Equipment.Count + 1);
        return tower.InstallEquipment(_equipmentTypes[typeName], serial, installedOn, mountHeight, tenancy);
    }
}
