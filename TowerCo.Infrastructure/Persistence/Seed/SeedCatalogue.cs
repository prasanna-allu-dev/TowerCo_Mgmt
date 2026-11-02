using TowerCo.Domain.Enums;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Infrastructure.Persistence.Seed;

/// <summary>Fictional demo portfolio. Data only; SeedData turns it into entities.</summary>
internal static class SeedCatalogue
{
    public const string FiveGAntenna = "5G Active Antenna (AAU)";
    public const string FourGRadio = "4G Remote Radio Unit";
    public const string DieselGenerator = "Diesel Generator";
    public const string BatteryBank = "Battery Bank";
    public const string SolarArray = "Solar Array";
    public const string MicrowaveDish = "Microwave Dish";

    public static readonly DateTime SeededAtUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly (string Name, EquipmentCategory Category)[] EquipmentTypes =
    [
        (FiveGAntenna, EquipmentCategory.Radio),
        (FourGRadio, EquipmentCategory.Radio),
        (MicrowaveDish, EquipmentCategory.Transmission),
        (DieselGenerator, EquipmentCategory.Power),
        (BatteryBank, EquipmentCategory.Power),
        (SolarArray, EquipmentCategory.Power),
        ("Air Conditioner", EquipmentCategory.Cooling),
        ("Security Camera", EquipmentCategory.Security)
    ];

    public static readonly (string Name, string CountryCode)[] Operators =
    [
        ("Aurora Mobile", "IN"), ("Bluewave Telecom", "IN"), ("Crestline Networks", "IN"),
        ("Delta Cell", "ZA"), ("Summit Wireless", "ZA"),
        ("Equinox Wireless", "KE"), ("Kilima Telecom", "KE"),
        ("Lagoon Mobile", "NG"), ("Savanna Connect", "NG")
    ];

    public static readonly Dictionary<string, Money> MonthlyRentByCountry = new()
    {
        ["IN"] = new Money(45000m, "INR"),
        ["ZA"] = new Money(9500m, "ZAR"),
        ["KE"] = new Money(60000m, "KES"),
        ["NG"] = new Money(350000m, "NGN")
    };

    public static readonly SiteSeed[] Sites =
    [
        new("IN-MUM-0001", "Andheri East Rooftop", new("IN", "Mumbai", 19.1136, 72.8697), SiteStatus.Active,
            new(TowerType.Rooftop, 18m, 3), [new("Aurora Mobile", D(2024, 4, 1)), new("Bluewave Telecom", D(2024, 9, 1)), new("Crestline Networks", D(2025, 2, 1))]),
        new("IN-DEL-0002", "Dwarka Sector 12", new("IN", "Delhi", 28.5921, 77.0460), SiteStatus.Active,
            new(TowerType.Monopole, 30m, 3), [new("Aurora Mobile", D(2023, 7, 1)), new("Bluewave Telecom", D(2025, 1, 15))]),
        new("IN-BLR-0003", "Whitefield Tech Park", new("IN", "Bengaluru", 12.9698, 77.7500), SiteStatus.Active,
            new(TowerType.Lattice, 45m, 4), [new("Aurora Mobile", D(2022, 1, 1), D(2025, 12, 31)), new("Crestline Networks", D(2024, 6, 1))]),
        new("IN-HYD-0004", "Gachibowli Junction", new("IN", "Hyderabad", 17.4401, 78.3489), SiteStatus.UnderConstruction,
            new(TowerType.Lattice, 40m, 4), []),
        new("ZA-JNB-0001", "Sandton Central", new("ZA", "Johannesburg", -26.1076, 28.0567), SiteStatus.Active,
            new(TowerType.Lattice, 50m, 3), [new("Delta Cell", D(2023, 3, 1)), new("Summit Wireless", D(2024, 8, 1))]),
        new("ZA-CPT-0002", "Bellville Ridge", new("ZA", "Cape Town", -33.9006, 18.6292), SiteStatus.Active,
            new(TowerType.Monopole, 25m, 2), [new("Delta Cell", D(2024, 2, 1)), new("Summit Wireless", D(2025, 5, 1))]),
        new("ZA-DUR-0003", "Umhlanga Heights", new("ZA", "Durban", -29.7277, 31.0711), SiteStatus.Planned,
            new(TowerType.Monopole, 30m, 2), []),
        new("KE-NBO-0001", "Westlands Hill", new("KE", "Nairobi", -1.2676, 36.8108), SiteStatus.Active,
            new(TowerType.Guyed, 60m, 3), [new("Equinox Wireless", D(2023, 10, 1)), new("Kilima Telecom", D(2024, 11, 1))]),
        new("KE-MBA-0002", "Nyali Beach Road", new("KE", "Mombasa", -4.0435, 39.7046), SiteStatus.Active,
            new(TowerType.Monopole, 28m, 2), [new("Equinox Wireless", D(2025, 3, 1))]),
        new("KE-KSM-0003", "Milimani Estate", new("KE", "Kisumu", -0.1022, 34.7617), SiteStatus.Decommissioned,
            new(TowerType.Lattice, 35m, 3), []),
        new("NG-LOS-0001", "Victoria Island Rooftop", new("NG", "Lagos", 6.4281, 3.4219), SiteStatus.Active,
            new(TowerType.Rooftop, 15m, 2), [new("Lagoon Mobile", D(2024, 1, 1)), new("Savanna Connect", D(2025, 7, 1))]),
        new("NG-ABV-0002", "Wuse II District", new("NG", "Abuja", 9.0765, 7.4790), SiteStatus.Active,
            new(TowerType.Lattice, 48m, 4), [new("Lagoon Mobile", D(2025, 9, 1))])
    ];

    private static DateOnly D(int year, int month, int day) => new(year, month, day);
}

internal sealed record SiteSeed(string Code, string Name, SiteLocation Location, SiteStatus Status, TowerSeed Tower, TenancySeed[] Tenancies);

internal sealed record TowerSeed(TowerType Type, decimal HeightMetres, int MaxTenants);

internal sealed record TenancySeed(string OperatorName, DateOnly Start, DateOnly? End = null);
