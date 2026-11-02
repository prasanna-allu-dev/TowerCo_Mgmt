using TowerCo.Application.Abstractions;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(TowerCoDbContext db) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
