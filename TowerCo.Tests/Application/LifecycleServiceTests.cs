using TowerCo.Application.Sites;
using TowerCo.Application.Tenancies;
using TowerCo.Application.Towers;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Tests.Application;

/// <summary>Site, tower and tenancy lifecycle commands through real services and a seeded database.</summary>
public class LifecycleServiceTests : IAsyncLifetime
{
    private static readonly CancellationToken Ct = CancellationToken.None;
    private SeededServices _services = null!;

    public async Task InitializeAsync() => _services = await SeededServices.CreateAsync();

    public async Task DisposeAsync() => await _services.DisposeAsync();

    private ISiteCommandService Sites => _services.Get<ISiteCommandService>();
    private ITenancyService Tenancies => _services.Get<ITenancyService>();

    private async Task<SiteDetailDto> NewPlannedSite() => await Sites.CreateAsync(new CreateSiteRequest
    {
        SiteCode = "IN-PUN-0009", Name = "Baner Road", CountryCode = "IN", City = "Pune",
        Latitude = 18.56, Longitude = 73.78, TowerType = TowerType.Monopole, HeightMetres = 35, MaxTenants = 1
    }, Ct);

    private async Task<SiteDetailDto> ActiveSiteWithFreeSlot()
    {
        var site = await NewPlannedSite();
        await Sites.ChangeStatusAsync(site.Id, new ChangeSiteStatusRequest { Status = SiteStatus.UnderConstruction }, Ct);
        return await Sites.ChangeStatusAsync(site.Id, new ChangeSiteStatusRequest { Status = SiteStatus.Active }, Ct);
    }

    private Task<TenancyDto> Lease(int towerId, DateOnly start, DateOnly? end = null) => Tenancies.AddAsync(towerId,
        new CreateTenancyRequest { OperatorId = 1, StartDate = start, EndDate = end, MonthlyRent = 1000m, Currency = "INR" }, Ct);

    [Fact]
    public async Task Site_MovesThroughLifecycle_AndPersists()
    {
        var site = await ActiveSiteWithFreeSlot();

        var reloaded = await _services.Get<ISiteQueryService>().GetAsync(site.Id, Ct);

        Assert.Equal(SiteStatus.Active, reloaded.Status);
    }

    [Fact]
    public async Task Site_SkippingStage_Throws()
    {
        var site = await NewPlannedSite();

        await Assert.ThrowsAsync<InvalidSiteTransitionException>(() =>
            Sites.ChangeStatusAsync(site.Id, new ChangeSiteStatusRequest { Status = SiteStatus.Active }, Ct));
    }

    [Fact]
    public async Task Site_DecommissionWithTenant_Throws_ThenSucceedsAfterEnd()
    {
        var site = await ActiveSiteWithFreeSlot();
        var tenancy = await Lease(site.TowerId, new DateOnly(2026, 1, 1));
        var decommission = new ChangeSiteStatusRequest { Status = SiteStatus.Decommissioned };

        await Assert.ThrowsAsync<SiteHasActiveTenantsException>(() => Sites.ChangeStatusAsync(site.Id, decommission, Ct));
        await Tenancies.EndAsync(site.TowerId, tenancy.Id, new EndTenancyRequest { EndDate = new DateOnly(2026, 2, 1) }, Ct);

        Assert.Equal(SiteStatus.Decommissioned, (await Sites.ChangeStatusAsync(site.Id, decommission, Ct)).Status);
    }

    [Fact]
    public async Task Site_Rename_Persists()
    {
        var site = await NewPlannedSite();

        Assert.Equal("Baner Hill", (await Sites.RenameAsync(site.Id, new RenameSiteRequest { Name = "Baner Hill" }, Ct)).Name);
    }

    [Fact]
    public async Task Site_Unknown_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => Sites.RenameAsync(9999, new RenameSiteRequest { Name = "X" }, Ct));

    [Fact]
    public async Task Tenancy_RenewAndRentReview_Persist()
    {
        var site = await ActiveSiteWithFreeSlot();
        var tenancy = await Lease(site.TowerId, new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));

        var renewed = await Tenancies.RenewAsync(site.TowerId, tenancy.Id, new RenewTenancyRequest { EndDate = new DateOnly(2027, 12, 31) }, Ct);
        var reviewed = await Tenancies.ReviewRentAsync(site.TowerId, tenancy.Id, new RentReviewRequest { MonthlyRent = 1200m }, Ct);

        Assert.Equal(new DateOnly(2027, 12, 31), renewed.EndDate);
        Assert.Equal((1200m, "INR"), (reviewed.MonthlyRent, reviewed.Currency));
    }

    [Fact]
    public async Task Tenancy_FutureBookingBlocksOverlap_OnSingleSlotTower()
    {
        var site = await ActiveSiteWithFreeSlot();
        await Lease(site.TowerId, new DateOnly(2027, 6, 1));

        await Assert.ThrowsAsync<TowerCapacityExceededException>(() =>
            Tenancies.AddAsync(site.TowerId, new CreateTenancyRequest
                { OperatorId = 2, StartDate = new DateOnly(2027, 1, 1), MonthlyRent = 900m, Currency = "INR" }, Ct));
    }

    [Fact]
    public async Task Tenancy_UnknownId_Throws()
    {
        var site = await ActiveSiteWithFreeSlot();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            Tenancies.EndAsync(site.TowerId, 9999, new EndTenancyRequest { EndDate = new DateOnly(2026, 1, 1) }, Ct));
    }

    [Fact]
    public async Task Tower_ChangeCapacity_UpAndBlockedBelowCommitments()
    {
        var site = await ActiveSiteWithFreeSlot();
        await Lease(site.TowerId, new DateOnly(2026, 1, 1));
        var towers = _services.Get<ITowerCommandService>();

        var upgraded = await towers.ChangeCapacityAsync(site.TowerId, new ChangeCapacityRequest { MaxTenants = 3 }, Ct);
        await Lease2(site.TowerId);

        Assert.Equal(3, upgraded.MaxTenants);
        await Assert.ThrowsAsync<CapacityBelowCommitmentsException>(() =>
            towers.ChangeCapacityAsync(site.TowerId, new ChangeCapacityRequest { MaxTenants = 1 }, Ct));
        await Assert.ThrowsAsync<NotFoundException>(() =>
            towers.ChangeCapacityAsync(9999, new ChangeCapacityRequest { MaxTenants = 2 }, Ct));
    }

    private Task<TenancyDto> Lease2(int towerId) => Tenancies.AddAsync(towerId,
        new CreateTenancyRequest { OperatorId = 2, StartDate = new DateOnly(2026, 1, 1), MonthlyRent = 800m, Currency = "INR" }, Ct);
}
