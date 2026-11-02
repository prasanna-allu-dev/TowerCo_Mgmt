using Microsoft.EntityFrameworkCore;
using TowerCo.Application.Abstractions;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class InspectionRepository(TowerCoDbContext db) : IInspectionRepository
{
    public async Task<IReadOnlyList<Inspection>> ListAsync(int? siteId, CancellationToken cancellationToken) =>
        await db.Inspections.AsNoTracking()
            .Where(i => siteId == null || i.SiteId == siteId)
            .ToListAsync(cancellationToken);
}
