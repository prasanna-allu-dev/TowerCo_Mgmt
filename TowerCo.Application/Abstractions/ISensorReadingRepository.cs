using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Abstractions;

public interface ISensorReadingRepository
{
    /// <summary>Readings recorded at or after the given time, optionally for one site.</summary>
    Task<IReadOnlyList<SensorReading>> ListSinceAsync(DateTime sinceUtc, string? siteCode, CancellationToken cancellationToken);
}
