using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Abstractions;

public interface IInspectionRepository
{
    Task<IReadOnlyList<Inspection>> ListAsync(int? siteId, CancellationToken cancellationToken);
}
