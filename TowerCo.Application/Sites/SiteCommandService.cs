using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Application.Sites;

public interface ISiteCommandService
{
    Task<SiteDetailDto> CreateAsync(CreateSiteRequest request, CancellationToken cancellationToken);
    Task<SiteDetailDto> ChangeStatusAsync(int siteId, ChangeSiteStatusRequest request, CancellationToken cancellationToken);
    Task<SiteDetailDto> RenameAsync(int siteId, RenameSiteRequest request, CancellationToken cancellationToken);
}

/// <summary>UC1: registers sites and manages their lifecycle. Rules live on the Site aggregate.</summary>
internal sealed class SiteCommandService(
    ISiteRepository sites, ITowerRepository towers, IUnitOfWork unitOfWork, TimeProvider timeProvider) : ISiteCommandService
{
    public async Task<SiteDetailDto> CreateAsync(CreateSiteRequest request, CancellationToken cancellationToken)
    {
        if (await sites.CodeExistsAsync(request.SiteCode, cancellationToken))
        {
            throw new DuplicateSiteCodeException(request.SiteCode);
        }
        var site = NewSite(request, timeProvider.GetUtcNow().UtcDateTime);
        var tower = site.AddTower(request.TowerType, request.HeightMetres, request.MaxTenants);
        sites.Add(site);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return SiteMapper.ToDetail(tower, timeProvider.Today());
    }

    public Task<SiteDetailDto> ChangeStatusAsync(int siteId, ChangeSiteStatusRequest request, CancellationToken cancellationToken) =>
        UpdateAsync(siteId, site => site.ChangeStatus(request.Status, timeProvider.Today()), cancellationToken);

    public Task<SiteDetailDto> RenameAsync(int siteId, RenameSiteRequest request, CancellationToken cancellationToken) =>
        UpdateAsync(siteId, site => site.Rename(request.Name), cancellationToken);

    private async Task<SiteDetailDto> UpdateAsync(int siteId, Action<Site> change, CancellationToken cancellationToken)
    {
        var tower = await towers.GetForUpdateBySiteIdAsync(siteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Site), siteId);
        change(tower.Site);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return SiteMapper.ToDetail(tower, timeProvider.Today());
    }

    private static Site NewSite(CreateSiteRequest request, DateTime createdAtUtc)
    {
        var location = new SiteLocation(request.CountryCode, request.City, request.Latitude, request.Longitude);
        return new Site(request.SiteCode, request.Name, location, SiteStatus.Planned, createdAtUtc);
    }
}
