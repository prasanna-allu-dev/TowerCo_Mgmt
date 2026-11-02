using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;
using static TowerCo.Tests.Domain.DomainBuilders;

namespace TowerCo.Tests.Domain;

public class TenancyAndEquipmentTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(-1, false)]
    public void IsActiveOn_EndDateRelativeToToday_ReturnsExpected(int? endOffsetDays, bool expected)
    {
        DateOnly? end = endOffsetDays is null ? null : Today.AddDays(endOffsetDays.Value);
        var tenancy = Tower().AddTenancy(Operator(), Today.AddYears(-1), end, Rent);

        Assert.Equal(expected, tenancy.IsActiveOn(Today));
    }

    [Fact]
    public void AddTenancy_EndBeforeStart_Throws()
    {
        Assert.Throws<DomainValidationException>(() => Tower().AddTenancy(Operator(), Today, Today.AddDays(-1), Rent));
    }

    [Fact]
    public void AddTenancy_ZeroRent_Throws()
    {
        Assert.Throws<DomainValidationException>(() => Tower().AddTenancy(Operator(), Today, null, new Money(0m, "INR")));
    }

    [Fact]
    public void Equipment_Remove_SetsRemovedStatusAndKeepsOwner()
    {
        var tower = Tower();
        var tenancy = tower.AddTenancy(Operator(), Today, null, Rent);
        var equipment = tower.InstallEquipment(new EquipmentType("Generator", EquipmentCategory.Power), "SN-2", Today, null, tenancy);

        equipment.Remove();

        Assert.Equal(EquipmentStatus.Removed, equipment.Status);
        Assert.Same(tenancy, equipment.Tenancy);
    }

    [Fact]
    public void Equipment_BlankSerial_Throws()
    {
        var tower = Tower();
        var tenancy = tower.AddTenancy(Operator(), Today, null, Rent);
        var type = new EquipmentType("Generator", EquipmentCategory.Power);

        Assert.Throws<DomainValidationException>(() => tower.InstallEquipment(type, "", Today, null, tenancy));
    }

    [Fact]
    public void Equipment_TenancyOnAnotherTower_Throws()
    {
        var otherTenancy = Tower().AddTenancy(Operator(), Today, null, Rent);
        var type = new EquipmentType("Dish", EquipmentCategory.Transmission);

        Assert.Throws<InvalidEquipmentOwnerException>(() => Tower().InstallEquipment(type, "SN-3", Today, null, otherTenancy));
    }

    [Fact]
    public void OperatorAndEquipmentType_ValidInput_SetProperties()
    {
        var mobileOperator = new Operator("Aurora", "IN");
        var type = new EquipmentType("Dish", EquipmentCategory.Transmission);

        Assert.Equal(("Aurora", "IN"), (mobileOperator.Name, mobileOperator.CountryCode));
        Assert.Equal(("Dish", EquipmentCategory.Transmission), (type.Name, type.Category));
    }

    [Fact]
    public void OperatorAndEquipmentType_BlankName_Throw()
    {
        Assert.Throws<DomainValidationException>(() => new Operator("", "IN"));
        Assert.Throws<DomainValidationException>(() => new Operator("Aurora", " "));
        Assert.Throws<DomainValidationException>(() => new EquipmentType("", EquipmentCategory.Radio));
    }
}
