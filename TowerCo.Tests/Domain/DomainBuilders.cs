using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Tests.Domain;

internal static class DomainBuilders
{
    public static readonly DateOnly Today = new(2026, 11, 2);
    public static readonly Money Rent = new(1000m, "INR");

    public static Site Site(SiteStatus status = SiteStatus.Active) =>
        new("IN-TST-0001", "Test Site", new SiteLocation("IN", "Pune", 18.5, 73.8), status, new DateTime(2026, 1, 1));

    public static Tower Tower(int maxTenants = 2) => Site().AddTower(TowerType.Monopole, 30m, maxTenants);

    public static Operator Operator(string name = "Test Mobile") => new(name, "IN");
}
