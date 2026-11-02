using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using static TowerCo.Tests.Domain.DomainBuilders;

namespace TowerCo.Tests.Domain;

public class TowerTests
{
    [Theory]
    [InlineData(0, 3)]
    [InlineData(10, 0)]
    public void AddTower_NonPositiveHeightOrCapacity_Throws(int height, int maxTenants)
    {
        Assert.Throws<DomainValidationException>(() => Site().AddTower(TowerType.Monopole, height, maxTenants));
    }

    [Fact]
    public void Capacity_NoTenancies_FullyAvailable()
    {
        var tower = Tower(maxTenants: 3);

        Assert.Equal(0, tower.ActiveTenantCount(Today));
        Assert.Equal(3, tower.AvailableSlots(Today));
        Assert.True(tower.HasCapacity(Today));
        Assert.Equal(0m, tower.UtilisationPercent(Today));
    }

    [Fact]
    public void Capacity_EndedTenancy_DoesNotUseSlot()
    {
        var tower = Tower(maxTenants: 3);
        tower.AddTenancy(Operator("A"), Today.AddYears(-2), Today.AddDays(-1), Rent);
        tower.AddTenancy(Operator("B"), Today.AddYears(-1), null, Rent);

        Assert.Equal(1, tower.ActiveTenantCount(Today));
        Assert.Equal(2, tower.AvailableSlots(Today));
        Assert.Equal(33.3m, tower.UtilisationPercent(Today));
    }

    [Fact]
    public void Capacity_AllSlotsTaken_HasNoCapacity()
    {
        var tower = Tower(maxTenants: 1);
        tower.AddTenancy(Operator(), Today, null, Rent);

        Assert.False(tower.HasCapacity(Today));
        Assert.Equal(0, tower.AvailableSlots(Today));
        Assert.Equal(100m, tower.UtilisationPercent(Today));
    }

    [Fact]
    public void AddTenancy_ValidInput_AddsLinkedTenancy()
    {
        var tower = Tower();
        var mobileOperator = Operator();

        var tenancy = tower.AddTenancy(mobileOperator, Today, Today.AddYears(5), Rent);

        Assert.Single(tower.Tenancies);
        Assert.Same(tower, tenancy.Tower);
        Assert.Same(mobileOperator, tenancy.Operator);
        Assert.Equal(Today, tenancy.StartDate);
        Assert.Equal(Today.AddYears(5), tenancy.EndDate);
        Assert.Equal(Rent, tenancy.MonthlyRent);
    }

    [Fact]
    public void InstallEquipment_WithTenancy_IsTenantOwnedAndInstalled()
    {
        var tower = Tower();
        var tenancy = tower.AddTenancy(Operator(), Today, null, Rent);
        var type = new EquipmentType("5G Antenna", EquipmentCategory.Radio);

        var equipment = tower.InstallEquipment(type, "SN-1", Today, 28m, tenancy);

        Assert.Single(tower.Equipment);
        Assert.Same(tower, equipment.Tower);
        Assert.Same(type, equipment.EquipmentType);
        Assert.Same(tenancy, equipment.Tenancy);
        Assert.Equal("SN-1", equipment.SerialNumber);
        Assert.Equal(Today, equipment.InstalledOn);
        Assert.Equal(28m, equipment.MountHeightMetres);
        Assert.Equal(EquipmentStatus.Installed, equipment.Status);
    }
}
