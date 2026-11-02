using Microsoft.EntityFrameworkCore;
using TowerCo.Application.Abstractions;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class SensorReadingRepository(TowerCoDbContext db) : ISensorReadingRepository
{
    public async Task<IReadOnlyList<SensorReading>> ListSinceAsync(DateTime sinceUtc, string? siteCode, CancellationToken cancellationToken) =>
        await db.SensorReadings.AsNoTracking()
            .Where(r => r.RecordedAtUtc >= sinceUtc && (siteCode == null || r.SiteCode == siteCode))
            .ToListAsync(cancellationToken);
}
