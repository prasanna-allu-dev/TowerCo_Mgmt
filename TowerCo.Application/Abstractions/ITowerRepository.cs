using TowerCo.Domain.Entities;

namespace TowerCo.Application.Abstractions;

/// <summary>Towers are loaded with their site, tenancies and operators (one tower per site).</summary>
public interface ITowerRepository
{
    Task<IReadOnlyList<Tower>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Read-only tower for a site, including equipment.</summary>
    Task<Tower?> GetBySiteIdAsync(int siteId, CancellationToken cancellationToken);

    /// <summary>Tracked tower so changes are saved by IUnitOfWork.</summary>
    Task<Tower?> GetForUpdateAsync(int towerId, CancellationToken cancellationToken);
}
