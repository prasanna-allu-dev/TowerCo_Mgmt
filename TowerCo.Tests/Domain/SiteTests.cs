using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;
using static TowerCo.Tests.Domain.DomainBuilders;

namespace TowerCo.Tests.Domain;

public class SiteTests
{
    [Fact]
    public void Constructor_ValidInput_SetsProperties()
    {
        var site = Site(SiteStatus.Planned);

        Assert.Equal("IN-TST-0001", site.SiteCode);
        Assert.Equal("Test Site", site.Name);
        Assert.Equal("Pune", site.Location.City);
        Assert.Equal(SiteStatus.Planned, site.Status);
        Assert.Equal(new DateTime(2026, 1, 1), site.CreatedAtUtc);
        Assert.Null(site.Tower);
    }

    [Theory]
    [InlineData("", "Name")]
    [InlineData("CODE", " ")]
    public void Constructor_BlankCodeOrName_Throws(string code, string name)
    {
        var location = new SiteLocation("IN", "Pune", 0, 0);

        Assert.Throws<DomainValidationException>(() => new Site(code, name, location, SiteStatus.Active, DateTime.UtcNow));
    }

    [Fact]
    public void AddTower_FirstTower_LinksBothWays()
    {
        var site = Site();

        var tower = site.AddTower(TowerType.Lattice, 45m, 4);

        Assert.Same(tower, site.Tower);
        Assert.Same(site, tower.Site);
        Assert.Equal(TowerType.Lattice, tower.TowerType);
        Assert.Equal(45m, tower.HeightMetres);
        Assert.Equal(4, tower.MaxTenants);
    }

    [Fact]
    public void AddTower_SecondTower_ThrowsTowerAlreadyExists()
    {
        var site = Site();
        site.AddTower(TowerType.Lattice, 45m, 4);

        var exception = Assert.Throws<TowerAlreadyExistsException>(() => site.AddTower(TowerType.Monopole, 20m, 2));

        Assert.Contains("IN-TST-0001", exception.Message);
    }
}
