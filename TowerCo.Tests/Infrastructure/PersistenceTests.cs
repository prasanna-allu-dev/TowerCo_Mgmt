using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TowerCo.Domain.Enums;
using TowerCo.Infrastructure.Persistence;
using TowerCo.Infrastructure.Persistence.Seed;

namespace TowerCo.Tests.Infrastructure;

public sealed class PersistenceTests : IAsyncLifetime
{
    private static readonly DateOnly Today = new(2026, 11, 2);
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        await DatabaseSeeder.SeedAsync(context);
    }

    public Task DisposeAsync() => _connection.DisposeAsync().AsTask();

    [Fact]
    public async Task Seed_EmptyDatabase_LoadsDemoPortfolio()
    {
        await using var context = CreateContext();

        Assert.Equal(SeedCatalogue.Sites.Length, await context.Sites.CountAsync());
        Assert.Equal(SeedCatalogue.Sites.Length, await context.Towers.CountAsync());
        Assert.Equal(SeedCatalogue.Operators.Length, await context.Operators.CountAsync());
        Assert.Equal(SeedCatalogue.EquipmentTypes.Length, await context.EquipmentTypes.CountAsync());
        Assert.Equal(SeedCatalogue.Sites.Sum(s => s.Tenancies.Length), await context.Tenancies.CountAsync());
        Assert.True(await context.Equipment.AnyAsync());
    }

    [Fact]
    public async Task Seed_RunTwice_DoesNotDuplicate()
    {
        await using var context = CreateContext();

        await DatabaseSeeder.SeedAsync(context);

        Assert.Equal(SeedCatalogue.Sites.Length, await context.Sites.CountAsync());
    }

    [Fact]
    public async Task Query_ByCountry_RoundTripsGraphAndValueObjects()
    {
        await using var context = CreateContext();

        var site = await context.Sites
            .Include(s => s.Tower!).ThenInclude(t => t.Tenancies).ThenInclude(t => t.Operator)
            .Include(s => s.Tower!).ThenInclude(t => t.Equipment).ThenInclude(e => e.EquipmentType)
            .SingleAsync(s => s.SiteCode == "IN-MUM-0001" && s.Location.CountryCode == "IN");

        Assert.Equal("Mumbai", site.Location.City);
        Assert.False(site.Tower!.HasCapacity(Today));
        Assert.Equal("INR", site.Tower.Tenancies.First().MonthlyRent.Currency);
        Assert.Contains(site.Tower.Equipment, e => e.Tenancy is null && e.EquipmentType.Category == EquipmentCategory.Power);
    }

    [Fact]
    public async Task Query_EndedTenancy_RadioMarkedRemovedAndSlotFree()
    {
        await using var context = CreateContext();

        var tower = await context.Towers.Include(t => t.Tenancies).Include(t => t.Equipment)
            .SingleAsync(t => t.Site.SiteCode == "IN-BLR-0003");

        Assert.Equal(1, tower.ActiveTenantCount(Today));
        Assert.Equal(2, tower.Equipment.Count(e => e.Status == EquipmentStatus.Removed));
    }

    [Theory]
    [InlineData("IN-MUM-0001", "Solar Array")]
    [InlineData("IN-DEL-0002", "Diesel Generator")]
    public async Task Seed_SharedPower_DependsOnTowerType(string siteCode, string expectedPower)
    {
        await using var context = CreateContext();

        var sharedNames = await context.Equipment
            .Where(e => e.Tower.Site.SiteCode == siteCode && e.TenancyId == null)
            .Select(e => e.EquipmentType.Name).ToListAsync();

        Assert.Equal(new[] { expectedPower, "Battery Bank" }.Order(), sharedNames.Order());
    }

    [Fact]
    public async Task Save_Graph_AssignsKeysAndForeignKeys()
    {
        await using var context = CreateContext();

        var equipment = await context.Equipment.Include(e => e.Tenancy).Include(e => e.Tower)
            .FirstAsync(e => e.TenancyId != null);

        Assert.True(equipment.Id > 0);
        Assert.Equal(equipment.Tenancy!.Id, equipment.TenancyId);
        Assert.Equal(equipment.Tower.Id, equipment.TowerId);
        Assert.True(equipment.EquipmentTypeId > 0);
        Assert.Equal(equipment.Tenancy!.TowerId, equipment.TowerId);
        Assert.True(equipment.Tenancy.OperatorId > 0);
        Assert.True(equipment.Tower.SiteId > 0);
    }

    private TowerCoDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<TowerCoDbContext>().UseSqlite(_connection).Options);
}
