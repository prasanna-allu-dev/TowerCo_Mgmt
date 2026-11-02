namespace TowerCo.Application.Sites;

public interface ISiteQueryService
{
    Task<IReadOnlyList<SiteSummaryDto>> ListAsync(SiteFilter filter, CancellationToken cancellationToken);

    Task<SiteDetailDto> GetAsync(int siteId, CancellationToken cancellationToken);
}
