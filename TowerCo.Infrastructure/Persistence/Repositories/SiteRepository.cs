using Microsoft.EntityFrameworkCore;
using TowerCo.Application.Abstractions;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class SiteRepository(TowerCoDbContext db) : ISiteRepository
{
    public Task<bool> CodeExistsAsync(string siteCode, CancellationToken cancellationToken) =>
        db.Sites.AnyAsync(s => s.SiteCode == siteCode, cancellationToken);

    public async Task<Site?> FindAsync(int id, CancellationToken cancellationToken) =>
        await db.Sites.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<Site>> ListAsync(CancellationToken cancellationToken) =>
        await db.Sites.AsNoTracking().OrderBy(s => s.SiteCode).ToListAsync(cancellationToken);

    public void Add(Site site) => db.Sites.Add(site);
}
