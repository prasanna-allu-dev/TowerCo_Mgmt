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
}

/// <summary>UC1: registers a site with its tower. Duplicate site codes are rejected (409).</summary>
internal sealed class SiteCommandService(ISiteRepository sites, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : ISiteCommandService
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

    private static Site NewSite(CreateSiteRequest request, DateTime createdAtUtc)
    {
        var location = new SiteLocation(request.CountryCode.ToUpperInvariant(), request.City, request.Latitude, request.Longitude);
        return new Site(request.SiteCode, request.Name, location, SiteStatus.Planned, createdAtUtc);
    }
}
