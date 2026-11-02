using Microsoft.Extensions.Logging;
using TowerCo.Ingestion.Contracts;

namespace TowerCo.Ingestion.Processing;

/// <summary>Demo publisher: writes alarms to the function log until the Service Bus queue exists.</summary>
public sealed class LoggingAlarmPublisher(ILogger<LoggingAlarmPublisher> logger) : IAlarmPublisher
{
    public Task PublishAsync(SensorAlarm alarm, CancellationToken cancellationToken)
    {
        logger.LogWarning("{Severity} alarm at {SiteCode}: {SensorType} = {Value}",
            alarm.Severity, alarm.Reading.SiteCode, alarm.Reading.SensorType, alarm.Reading.Value);
        return Task.CompletedTask;
    }
}
