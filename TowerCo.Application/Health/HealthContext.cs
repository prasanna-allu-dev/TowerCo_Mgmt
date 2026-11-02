using TowerCo.Domain.Entities;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Health;

/// <summary>Everything a rule needs to score one site.</summary>
public sealed record HealthContext(
    Site Site, IReadOnlyList<SensorReading> LatestReadings, Inspection? LastInspection,
    IReadOnlyList<WorkOrder> OpenWorkOrders, DateOnly Today);

/// <summary>One health signal. Add a signal by adding a rule (Open/Closed).</summary>
public interface IHealthSignalRule
{
    HealthSignal Evaluate(HealthContext context);
}
