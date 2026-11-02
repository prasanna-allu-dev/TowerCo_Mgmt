using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Application.Sites;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Towers;

public sealed record TowerConfigurationDto(
    int TowerId, int SiteId, string SiteCode, string SiteName, string CountryCode, SiteStatus SiteStatus,
    TowerType TowerType, decimal HeightMetres, CapacitySummaryDto Capacity);

public interface ITowerQueryService
{
    Task<IReadOnlyList<TowerConfigurationDto>> ListAsync(CancellationToken cancellationToken);
}

internal sealed class TowerQueryService(ITowerRepository towers, TimeProvider timeProvider) : ITowerQueryService
{
    public async Task<IReadOnlyList<TowerConfigurationDto>> ListAsync(CancellationToken cancellationToken)
    {
        var today = timeProvider.Today();
        var all = await towers.ListAsync(cancellationToken);
        return all.Select(t => ToConfiguration(t, today)).ToList();
    }

    private static TowerConfigurationDto ToConfiguration(Tower tower, DateOnly today) => new(
        tower.Id, tower.SiteId, tower.Site.SiteCode, tower.Site.Name, tower.Site.Location.CountryCode,
        tower.Site.Status, tower.TowerType, tower.HeightMetres, SiteMapper.ToCapacity(tower, today));
}
