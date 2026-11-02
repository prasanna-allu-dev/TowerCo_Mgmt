using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Application.Sites;

internal sealed class SiteQueryService(ITowerRepository towers, TimeProvider timeProvider) : ISiteQueryService
{
    public async Task<IReadOnlyList<SiteSummaryDto>> ListAsync(SiteFilter filter, CancellationToken cancellationToken)
    {
        var today = timeProvider.Today();
        var all = await towers.ListAsync(cancellationToken);
        return all.Where(t => Matches(t.Site, filter)).Select(t => SiteMapper.ToSummary(t, today)).ToList();
    }

    public async Task<SiteDetailDto> GetAsync(int siteId, CancellationToken cancellationToken)
    {
        var tower = await towers.GetBySiteIdAsync(siteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Site), siteId);
        return SiteMapper.ToDetail(tower, timeProvider.Today());
    }

    private static bool Matches(Site site, SiteFilter filter) =>
        (filter.Country is null || site.Location.CountryCode.Equals(filter.Country, StringComparison.OrdinalIgnoreCase))
        && (filter.Status is null || site.Status == filter.Status)
        && MatchesText(site, filter.Search);

    private static bool MatchesText(Site site, string? search) =>
        string.IsNullOrWhiteSpace(search)
        || new[] { site.SiteCode, site.Name, site.Location.City }
            .Any(value => value.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));
}
