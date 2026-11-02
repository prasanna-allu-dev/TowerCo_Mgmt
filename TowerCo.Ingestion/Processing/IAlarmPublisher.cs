using TowerCo.Ingestion.Contracts;

namespace TowerCo.Ingestion.Processing;

/// <summary>Hands alarms to the work order side. Swap the logging stub for Service Bus without touching callers.</summary>
public interface IAlarmPublisher
{
    Task PublishAsync(SensorAlarm alarm, CancellationToken cancellationToken);
}
