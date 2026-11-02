using TowerCo.Domain.Entities;

namespace TowerCo.Application.Abstractions;

public interface IOperatorRepository
{
    Task<IReadOnlyList<Operator>> ListAsync(CancellationToken cancellationToken);

    Task<Operator?> FindAsync(int id, CancellationToken cancellationToken);
}
