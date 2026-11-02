using TowerCo.Domain.Entities;

namespace TowerCo.Application.Abstractions;

public interface ISiteRepository
{
    Task<bool> CodeExistsAsync(string siteCode, CancellationToken cancellationToken);
    Task<Site?> FindAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Site>> ListAsync(CancellationToken cancellationToken);
    void Add(Site site);
}
