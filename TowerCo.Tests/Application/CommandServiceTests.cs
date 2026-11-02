using TowerCo.Application.Sites;
using TowerCo.Application.WorkOrders;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Tests.Application;

public class CommandServiceTests : IAsyncLifetime
{
    private SeededServices _services = null!;

    public async Task InitializeAsync() => _services = await SeededServices.CreateAsync();

    public async Task DisposeAsync() => await _services.DisposeAsync();

    private static CreateSiteRequest NewSite(string code = "IN-PUN-0005") => new()
    {
        SiteCode = code, Name = "Hinjewadi Phase 2", CountryCode = "in", City = "Pune",
        Latitude = 18.59, Longitude = 73.74, TowerType = TowerType.Monopole, HeightMetres = 40, MaxTenants = 3
    };

    private static CreateWorkOrderRequest NewWorkOrder(int siteId, string? assignee = null) => new()
    {
        SiteId = siteId, Title = "Generator service", Category = WorkOrderCategory.Preventive,
        Priority = WorkOrderPriority.Low, DueDate = new DateOnly(2026, 12, 1), Assignee = assignee
    };

    [Fact]
    public async Task CreateSite_NewCode_IsPlannedWithEmptyTower()
    {
        var site = await _services.Get<ISiteCommandService>().CreateAsync(NewSite(), CancellationToken.None);

        Assert.Equal(SiteStatus.Planned, site.Status);
        Assert.Equal("IN", site.CountryCode);
        Assert.Equal(3, site.Capacity.AvailableSlots);
        Assert.Empty(site.Tenancies);
    }

    [Fact]
    public async Task CreateSite_DuplicateCode_Throws()
    {
        var existing = (await _services.Get<ISiteQueryService>().ListAsync(new SiteFilter(), CancellationToken.None))[0];

        await Assert.ThrowsAsync<DuplicateSiteCodeException>(() =>
            _services.Get<ISiteCommandService>().CreateAsync(NewSite(existing.SiteCode), CancellationToken.None));
    }

    [Fact]
    public async Task WorkOrder_CreateThenAssign_PersistsAcrossScopes()
    {
        var created = await _services.Get<IWorkOrderService>().CreateAsync(NewWorkOrder(1), CancellationToken.None);
        var request = new UpdateWorkOrderStatusRequest { Status = WorkOrderStatus.Assigned, Assignee = "Priya Nair" };

        await _services.Get<IWorkOrderService>().UpdateStatusAsync(created.Id, request, CancellationToken.None);
        var reloaded = await _services.Get<IWorkOrderService>().GetAsync(created.Id, CancellationToken.None);

        Assert.Equal($"WO-{created.Id:D4}", reloaded.Code);
        Assert.Equal(WorkOrderStatus.Assigned, reloaded.Status);
        Assert.Equal("Priya Nair", reloaded.Assignee);
    }

    [Fact]
    public async Task WorkOrder_UnknownSiteOrId_ThrowsNotFound()
    {
        var service = _services.Get<IWorkOrderService>();

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(NewWorkOrder(9999), CancellationToken.None));
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetAsync(9999, CancellationToken.None));
    }

    [Fact]
    public async Task WorkOrder_ListFilters_Narrow()
    {
        var service = _services.Get<IWorkOrderService>();
        var first = (await service.ListAsync(new WorkOrderFilter(), CancellationToken.None))[0];

        var result = await service.ListAsync(new WorkOrderFilter
        {
            Search = first.Code, Status = first.Status, Priority = first.Priority, Country = first.CountryCode
        }, CancellationToken.None);

        Assert.Equal(first.Id, Assert.Single(result).Id);
    }
}
