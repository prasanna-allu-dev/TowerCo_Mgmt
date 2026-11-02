using TowerCo.Application.Constants;
using TowerCo.Application.EquipmentRegister;
using TowerCo.Application.Tenants;
using TowerCo.Domain.Enums;

namespace TowerCo.Tests.Application;

public class RegisterQueryTests : IAsyncLifetime
{
    private SeededServices _services = null!;

    public async Task InitializeAsync() => _services = await SeededServices.CreateAsync();

    public async Task DisposeAsync() => await _services.DisposeAsync();

    private Task<IReadOnlyList<TenantRecordDto>> Tenants(TenantFilter filter) =>
        _services.Get<ITenantQueryService>().ListAsync(filter, CancellationToken.None);

    private Task<IReadOnlyList<EquipmentItemDto>> Equipment(EquipmentFilter filter) =>
        _services.Get<IEquipmentQueryService>().ListAsync(filter, CancellationToken.None);

    [Fact]
    public async Task Tenants_StateFilter_SplitsActiveAndEnded()
    {
        var all = await Tenants(new TenantFilter());
        var active = await Tenants(new TenantFilter { State = FilterValues.TenancyActive });
        var ended = await Tenants(new TenantFilter { State = FilterValues.TenancyEnded });

        Assert.All(active, r => Assert.True(r.IsActive));
        Assert.All(ended, r => Assert.False(r.IsActive));
        Assert.Equal(all.Count, active.Count + ended.Count);
    }

    [Fact]
    public async Task Tenants_OperatorCountryAndSearch_Narrow()
    {
        var first = (await Tenants(new TenantFilter()))[0];

        var result = await Tenants(new TenantFilter
        {
            OperatorId = first.OperatorId, Country = first.CountryCode.ToLowerInvariant(), Search = first.SiteCode
        });

        Assert.Contains(result, r => r.TenancyId == first.TenancyId);
        Assert.All(result, r => Assert.Equal(first.OperatorId, r.OperatorId));
    }

    [Fact]
    public async Task Equipment_EveryItem_HasTenantOwnerFoundBySearch()
    {
        var all = await Equipment(new EquipmentFilter());
        var first = all[0];

        var byOwner = await Equipment(new EquipmentFilter { Search = first.OwnerName });

        Assert.All(all, e => Assert.False(string.IsNullOrWhiteSpace(e.OwnerName)));
        Assert.Contains(byOwner, e => e.Id == first.Id);
    }

    [Fact]
    public async Task Equipment_CategoryStatusCountrySiteAndSearch_Narrow()
    {
        var first = (await Equipment(new EquipmentFilter { Status = EquipmentStatus.Installed }))[0];

        var result = await Equipment(new EquipmentFilter
        {
            Category = first.Category, Status = first.Status, Country = first.CountryCode,
            SiteId = first.SiteId, Search = first.SerialNumber
        });

        Assert.Equal(first.Id, Assert.Single(result).Id);
    }

    [Fact]
    public async Task EquipmentTypes_ItemCount_MatchesInstalledItems()
    {
        var types = await _services.Get<IEquipmentQueryService>().ListTypesAsync(CancellationToken.None);
        var removed = await Equipment(new EquipmentFilter { Status = EquipmentStatus.Removed });
        var all = await Equipment(new EquipmentFilter());

        Assert.Equal(all.Count - removed.Count, types.Sum(t => t.ItemCount));
    }
}
