using Microsoft.Extensions.Options;
using TowerCo.Domain.Monitoring;
using TowerCo.Ingestion.Contracts;
using TowerCo.Ingestion.Exceptions;

namespace TowerCo.Ingestion.Processing;

/// <summary>Normalises a reading, checks it against its threshold and publishes an alarm when breached.</summary>
public sealed class SensorReadingProcessor(IOptions<AlarmThresholdOptions> options, IAlarmPublisher publisher)
    : ISensorReadingProcessor
{
    public async Task<ReadingOutcome> ProcessAsync(SensorReadingMessage message, CancellationToken cancellationToken)
    {
        var reading = new SensorReading(message.SiteCode, message.SensorType, message.Value, message.RecordedAtUtc);
        var threshold = FindThreshold(reading.SensorType);
        var severity = threshold.Evaluate(reading.Value);
        if (severity != AlarmSeverity.None)
        {
            await publisher.PublishAsync(new SensorAlarm(reading, severity, threshold), cancellationToken);
        }
        return new ReadingOutcome(reading.SiteCode, reading.SensorType, reading.Value, severity);
    }

    private AlarmThreshold FindThreshold(SensorType sensorType) =>
        options.Value.TryGetValue(sensorType, out var threshold)
            ? threshold
            : throw new MissingAlarmThresholdException(sensorType);
}
