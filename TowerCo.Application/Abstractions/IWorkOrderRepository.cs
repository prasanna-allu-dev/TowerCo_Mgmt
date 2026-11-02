using TowerCo.Domain.Entities;

namespace TowerCo.Application.Abstractions;

public interface IWorkOrderRepository
{
    Task<IReadOnlyList<WorkOrder>> ListAsync(CancellationToken cancellationToken);
    Task<WorkOrder?> GetAsync(int id, CancellationToken cancellationToken);
    void Add(WorkOrder workOrder);
}
