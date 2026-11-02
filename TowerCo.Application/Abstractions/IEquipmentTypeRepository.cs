using TowerCo.Domain.Entities;

namespace TowerCo.Application.Abstractions;

public interface IEquipmentTypeRepository
{
    Task<IReadOnlyList<EquipmentType>> ListAsync(CancellationToken cancellationToken);
}
