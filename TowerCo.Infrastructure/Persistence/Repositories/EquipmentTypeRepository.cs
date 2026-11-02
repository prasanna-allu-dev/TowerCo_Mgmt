using Microsoft.EntityFrameworkCore;
using TowerCo.Application.Abstractions;
using TowerCo.Domain.Entities;

namespace TowerCo.Infrastructure.Persistence.Repositories;

public sealed class EquipmentTypeRepository(TowerCoDbContext db) : IEquipmentTypeRepository
{
    public async Task<IReadOnlyList<EquipmentType>> ListAsync(CancellationToken cancellationToken) =>
        await db.EquipmentTypes.AsNoTracking().OrderBy(t => t.Category).ThenBy(t => t.Name).ToListAsync(cancellationToken);
}
