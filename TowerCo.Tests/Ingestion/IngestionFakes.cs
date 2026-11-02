using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TowerCo.Domain.Monitoring;
using TowerCo.Ingestion.Contracts;
using TowerCo.Ingestion.Processing;

namespace TowerCo.Tests.Ingestion;

internal sealed class RecordingAlarmPublisher : IAlarmPublisher
{
    public List<SensorAlarm> Published { get; } = [];

    public Task PublishAsync(SensorAlarm alarm, CancellationToken cancellationToken)
    {
        Published.Add(alarm);
        return Task.CompletedTask;
    }
}

internal sealed class RecordingLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, string Message)> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter) => Entries.Add((logLevel, formatter(state, exception)));
}

internal static class IngestionBuilders
{
    public const string SiteCode = "IN-MUM-0001";

    public static readonly DateTime RecordedAt = new(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);

    public static IOptions<AlarmThresholdOptions> Thresholds() => Options.Create(new AlarmThresholdOptions
    {
        [SensorType.ShelterTemperature] = new AlarmThreshold(35m, 45m, ThresholdDirection.HighIsBad)
    });

    public static SensorReadingProcessor Processor(RecordingAlarmPublisher publisher) => new(Thresholds(), publisher);

    public static string Json(string sensorType, decimal value, string siteCode = SiteCode) =>
        $$"""{ "siteCode": "{{siteCode}}", "sensorType": "{{sensorType}}", "value": {{value}}, "recordedAtUtc": "2026-11-02T09:00:00Z" }""";
}
