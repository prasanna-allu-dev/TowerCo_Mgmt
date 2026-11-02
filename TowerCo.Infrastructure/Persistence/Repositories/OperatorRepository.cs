using Microsoft.EntityFrameworkCore;
using TowerCo.Application.Abstractions;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class OperatorRepository(TowerCoDbContext db) : IOperatorRepository
{
    public async Task<IReadOnlyList<Operator>> ListAsync(CancellationToken cancellationToken) =>
        await db.Operators.AsNoTracking().OrderBy(o => o.Name).ToListAsync(cancellationToken);

    public async Task<Operator?> FindAsync(int id, CancellationToken cancellationToken) =>
        await db.Operators.FindAsync([id], cancellationToken);
}
