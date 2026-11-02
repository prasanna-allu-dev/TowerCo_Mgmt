using Microsoft.EntityFrameworkCore;
using TowerCo.Application.Abstractions;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class TowerRepository(TowerCoDbContext db) : ITowerRepository
{
    public async Task<IReadOnlyList<Tower>> ListAsync(CancellationToken cancellationToken) =>
        await WithGraph().AsNoTrackingWithIdentityResolution().ToListAsync(cancellationToken);

    /// <summary>Identity resolution links each equipment item to its owning tenancy loaded in the same graph.</summary>
    public Task<Tower?> GetBySiteIdAsync(int siteId, CancellationToken cancellationToken) =>
        WithGraph().AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(t => t.SiteId == siteId, cancellationToken);

    public Task<Tower?> GetForUpdateAsync(int towerId, CancellationToken cancellationToken) =>
        WithGraph().FirstOrDefaultAsync(t => t.Id == towerId, cancellationToken);

    public Task<Tower?> GetForUpdateBySiteIdAsync(int siteId, CancellationToken cancellationToken) =>
        WithGraph().FirstOrDefaultAsync(t => t.SiteId == siteId, cancellationToken);

    private IQueryable<Tower> WithGraph() => db.Towers
        .Include(t => t.Site)
        .Include(t => t.Tenancies).ThenInclude(t => t.Operator)
        .Include(t => t.Equipment).ThenInclude(e => e.EquipmentType);
}
