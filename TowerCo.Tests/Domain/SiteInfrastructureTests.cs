using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using static TowerCo.Tests.Domain.DomainBuilders;

namespace TowerCo.Tests.Domain;

public class SiteInfrastructureTests
{
    [Fact]
    public void InstallInfrastructure_AddsTowerCoKit_WithoutUsingTenantSlots()
    {
        var tower = Tower(maxTenants: 2);

        var item = tower.InstallInfrastructure(SiteInfrastructureKind.Generator, "GEN-001", Today);

        Assert.Same(tower, item.Tower);
        Assert.Equal((SiteInfrastructureKind.Generator, "GEN-001", Today), (item.Kind, item.SerialNumber, item.InstalledOn));
        Assert.Single(tower.Infrastructure);
        Assert.Equal(2, tower.AvailableSlots(Today));
    }

    [Fact]
    public void InstallInfrastructure_BlankSerial_Throws() =>
        Assert.Throws<DomainValidationException>(() => Tower().InstallInfrastructure(SiteInfrastructureKind.Battery, " ", Today));
}
