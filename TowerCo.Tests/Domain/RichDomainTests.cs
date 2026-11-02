using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;
using static TowerCo.Tests.Domain.DomainBuilders;

namespace TowerCo.Tests.Domain;

public class MoneyAndLocationTests
{
    [Fact]
    public void Money_NormalisesCurrency_AndAnnualises()
    {
        var rent = new Money(1000m, "inr");

        Assert.Equal(new Money(12000m, "INR"), rent.Annualised());
    }

    [Theory]
    [InlineData(-1, "INR")]
    [InlineData(10, "RUPEE")]
    [InlineData(10, "")]
    public void Money_InvalidValues_Throw(decimal amount, string currency) =>
        Assert.Throws<DomainValidationException>(() => new Money(amount, currency));

    [Fact]
    public void Money_Add_SameCurrency_Sums() =>
        Assert.Equal(new Money(30m, "KES"), new Money(10m, "KES").Add(new Money(20m, "KES")));

    [Fact]
    public void Money_Add_DifferentCurrency_Throws() =>
        Assert.Throws<CurrencyMismatchException>(() => new Money(10m, "KES").Add(new Money(10m, "INR")));

    [Theory]
    [InlineData("IND", "Pune", 0, 0)]
    [InlineData("IN", " ", 0, 0)]
    [InlineData("IN", "Pune", 91, 0)]
    [InlineData("IN", "Pune", 0, -181)]
    public void SiteLocation_InvalidValues_Throw(string country, string city, double lat, double lon) =>
        Assert.Throws<DomainValidationException>(() => new SiteLocation(country, city, lat, lon));

    [Fact]
    public void SiteLocation_NormalisesCountryAndCity()
    {
        var location = new SiteLocation(" in ", " Pune ", 18.5, 73.8);

        Assert.Equal(("IN", "Pune"), (location.CountryCode, location.City));
    }
}

public class SiteLifecycleTests
{
    [Fact]
    public void ChangeStatus_FullLifecycle_Succeeds()
    {
        var site = Site(SiteStatus.Planned);
        site.AddTower(TowerType.Monopole, 30m, 2);

        site.ChangeStatus(SiteStatus.UnderConstruction, Today);
        site.ChangeStatus(SiteStatus.Active, Today);
        site.ChangeStatus(SiteStatus.Decommissioned, Today);

        Assert.Equal(SiteStatus.Decommissioned, site.Status);
    }

    [Theory]
    [InlineData(SiteStatus.Planned, SiteStatus.Active)]
    [InlineData(SiteStatus.Active, SiteStatus.Planned)]
    [InlineData(SiteStatus.Decommissioned, SiteStatus.Active)]
    public void ChangeStatus_SkippingOrReversing_Throws(SiteStatus from, SiteStatus to) =>
        Assert.Throws<InvalidSiteTransitionException>(() => Site(from).ChangeStatus(to, Today));

    [Fact]
    public void Activate_WithoutTower_Throws() =>
        Assert.Throws<SiteHasNoTowerException>(() => Site(SiteStatus.UnderConstruction).ChangeStatus(SiteStatus.Active, Today));

    [Fact]
    public void Decommission_WithActiveTenant_Throws()
    {
        var tower = Tower();
        tower.AddTenancy(Operator(), Today.AddDays(-10), null, Rent);

        var exception = Assert.Throws<SiteHasActiveTenantsException>(() => tower.Site.ChangeStatus(SiteStatus.Decommissioned, Today));

        Assert.Contains("1 active or future", exception.Message);
    }

    [Fact]
    public void Decommission_AfterTenancyEnded_Succeeds()
    {
        var tower = Tower();
        var tenancy = tower.AddTenancy(Operator(), Today.AddDays(-10), null, Rent);
        tower.EndTenancy(tenancy, Today.AddDays(-1));

        tower.Site.ChangeStatus(SiteStatus.Decommissioned, Today);

        Assert.Equal(SiteStatus.Decommissioned, tower.Site.Status);
    }

    [Fact]
    public void Rename_TrimsAndRejectsBlank()
    {
        var site = Site();
        site.Rename("  New Name ");

        Assert.Equal("New Name", site.Name);
        Assert.Throws<DomainValidationException>(() => site.Rename(" "));
    }
}

public class TenancyLifecycleTests
{
    [Fact]
    public void FutureLease_IsNotActiveYet_ButIsCommitted()
    {
        var tenancy = Tower().AddTenancy(Operator(), Today.AddDays(30), null, Rent);

        Assert.False(tenancy.IsActiveOn(Today));
        Assert.True(tenancy.IsCommittedFrom(Today));
    }

    [Fact]
    public void FixedTerm_ReportsExpiryAndAnnualValue()
    {
        var tenancy = Tower().AddTenancy(Operator(), Today.AddDays(-30), Today.AddDays(45), Rent);

        Assert.Equal(45, tenancy.DaysToExpiry(Today));
        Assert.True(tenancy.IsExpiringWithin(Today, 90));
        Assert.False(tenancy.IsExpiringWithin(Today, 30));
        Assert.Equal(new Money(12000m, "INR"), tenancy.AnnualValue);
    }

    [Fact]
    public void OpenEnded_HasNoExpiry()
    {
        var tenancy = Tower().AddTenancy(Operator(), Today, null, Rent);

        Assert.True(tenancy.IsOpenEnded);
        Assert.Null(tenancy.DaysToExpiry(Today));
        Assert.False(tenancy.IsExpiringWithin(Today, 90));
    }

    [Fact]
    public void EndTenancy_FreesSlotAfterEndDate()
    {
        var tower = Tower(maxTenants: 1);
        var tenancy = tower.AddTenancy(Operator(), Today.AddDays(-10), null, Rent);

        tower.EndTenancy(tenancy, Today);

        Assert.True(tower.HasCapacity(Today.AddDays(1)));
        Assert.NotNull(tower.AddTenancy(Operator("Next"), Today.AddDays(1), null, Rent));
    }

    [Fact]
    public void EndTenancy_InvalidDates_Throw()
    {
        var tower = Tower();
        var tenancy = tower.AddTenancy(Operator(), Today, Today.AddDays(30), Rent);

        Assert.Throws<InvalidTenancyChangeException>(() => tower.EndTenancy(tenancy, Today.AddDays(-1)));
        Assert.Throws<InvalidTenancyChangeException>(() => tower.EndTenancy(tenancy, Today.AddDays(60)));
    }

    [Fact]
    public void Renew_ExtendsFixedTerm()
    {
        var tower = Tower();
        var tenancy = tower.AddTenancy(Operator(), Today, Today.AddDays(30), Rent);

        tower.RenewTenancy(tenancy, Today.AddDays(395));

        Assert.Equal(Today.AddDays(395), tenancy.EndDate);
    }

    [Fact]
    public void Renew_InvalidRequests_Throw()
    {
        var tower = Tower();
        var openEnded = tower.AddTenancy(Operator("Open"), Today, null, Rent);
        var fixedTerm = tower.AddTenancy(Operator("Fixed"), Today, Today.AddDays(30), Rent);

        Assert.Throws<InvalidTenancyChangeException>(() => tower.RenewTenancy(openEnded, Today.AddDays(90)));
        Assert.Throws<InvalidTenancyChangeException>(() => tower.RenewTenancy(fixedTerm, Today.AddDays(10)));
    }

    [Fact]
    public void Renew_IntoSlotLeasedToSomeoneElse_Throws()
    {
        var tower = Tower(maxTenants: 1);
        var current = tower.AddTenancy(Operator("Current"), Today, Today.AddDays(30), Rent);
        tower.AddTenancy(Operator("Next"), Today.AddDays(31), null, Rent);

        Assert.Throws<TowerCapacityExceededException>(() => tower.RenewTenancy(current, Today.AddDays(60)));
    }

    [Fact]
    public void ReviewRent_SameCurrency_Updates_DifferentCurrency_Throws()
    {
        var tower = Tower();
        var tenancy = tower.AddTenancy(Operator(), Today, null, Rent);

        tower.ReviewRent(tenancy, new Money(1100m, "INR"));

        Assert.Equal(1100m, tenancy.MonthlyRent.Amount);
        Assert.Throws<CurrencyMismatchException>(() => tower.ReviewRent(tenancy, new Money(1100m, "USD")));
    }

    [Fact]
    public void TenancyFromAnotherTower_IsRejected() =>
        Assert.Throws<NotFoundException>(() => Tower().EndTenancy(Tower().AddTenancy(Operator(), Today, null, Rent), Today));
}

public class TowerCapacityOverTimeTests
{
    [Fact]
    public void FutureLease_BlocksOverlappingLeaseWhenFull()
    {
        var tower = Tower(maxTenants: 1);
        tower.AddTenancy(Operator("Booked"), Today.AddDays(30), null, Rent);

        Assert.Throws<TowerCapacityExceededException>(() => tower.AddTenancy(Operator("Now"), Today, null, Rent));
    }

    [Fact]
    public void ShortLeaseBeforeFutureLease_Fits()
    {
        var tower = Tower(maxTenants: 1);
        tower.AddTenancy(Operator("Booked"), Today.AddDays(30), null, Rent);

        Assert.NotNull(tower.AddTenancy(Operator("Short"), Today, Today.AddDays(29), Rent));
    }

    [Fact]
    public void SameOperator_OverlappingFuturePeriod_IsDuplicate()
    {
        var tower = Tower();
        var mobileOperator = Operator();
        tower.AddTenancy(mobileOperator, Today, Today.AddDays(30), Rent);

        Assert.Throws<DuplicateTenancyException>(() => tower.AddTenancy(mobileOperator, Today.AddDays(10), null, Rent));
        Assert.NotNull(tower.AddTenancy(mobileOperator, Today.AddDays(31), null, Rent));
    }

    [Fact]
    public void ChangeCapacity_UpgradeAndSafeDowngrade()
    {
        var tower = Tower(maxTenants: 3);
        tower.AddTenancy(Operator("A"), Today, null, Rent);
        tower.AddTenancy(Operator("B"), Today.AddDays(10), null, Rent);

        tower.ChangeCapacity(5, Today);
        Assert.Equal(5, tower.MaxTenants);
        tower.ChangeCapacity(2, Today);
        Assert.Equal(2, tower.MaxTenants);
    }

    [Fact]
    public void ChangeCapacity_BelowCommitted_Throws()
    {
        var tower = Tower(maxTenants: 3);
        tower.AddTenancy(Operator("A"), Today, null, Rent);
        tower.AddTenancy(Operator("B"), Today.AddDays(10), null, Rent);

        var exception = Assert.Throws<CapacityBelowCommitmentsException>(() => tower.ChangeCapacity(1, Today));

        Assert.Contains("2 are committed", exception.Message);
        Assert.Throws<DomainValidationException>(() => tower.ChangeCapacity(0, Today));
    }

    [Fact]
    public void MonthlyRevenue_SumsActiveTenanciesPerCurrency()
    {
        var tower = Tower(maxTenants: 4);
        tower.AddTenancy(Operator("A"), Today.AddDays(-5), null, Rent);
        tower.AddTenancy(Operator("B"), Today.AddDays(-5), null, new Money(500m, "INR"));
        tower.AddTenancy(Operator("C"), Today.AddDays(5), null, Rent);

        Assert.Equal([new Money(1500m, "INR")], tower.MonthlyRevenue(Today));
    }
}
