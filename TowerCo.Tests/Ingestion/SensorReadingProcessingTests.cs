using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.Monitoring;
using TowerCo.Ingestion;
using TowerCo.Ingestion.Contracts;
using TowerCo.Ingestion.Exceptions;
using TowerCo.Ingestion.Processing;
using static TowerCo.Tests.Ingestion.IngestionBuilders;

namespace TowerCo.Tests.Ingestion;

public class SensorReadingProcessingTests
{
    private readonly SensorReadingParser _parser = new();
    private readonly RecordingAlarmPublisher _publisher = new();

    [Fact]
    public void Parse_ValidJson_ReadsEnumAndValues()
    {
        var message = _parser.Parse(Json("ShelterTemperature", 40.5m));

        Assert.Equal(new SensorReadingMessage(SiteCode, SensorType.ShelterTemperature, 40.5m, RecordedAt), message);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{ "sensorType": "Unknown" }""")]
    [InlineData("null")]
    public void Parse_UnreadableBody_ThrowsInvalidPayload(string body)
    {
        Assert.Throws<InvalidSensorPayloadException>(() => _parser.Parse(body));
    }

    [Fact]
    public async Task Process_WithinThreshold_ReturnsNoneAndPublishesNothing()
    {
        var outcome = await Processor(_publisher).ProcessAsync(Message(30m), CancellationToken.None);

        Assert.Equal(AlarmSeverity.None, outcome.Severity);
        Assert.Empty(_publisher.Published);
    }

    [Fact]
    public async Task Process_Breach_PublishesAlarmWithThreshold()
    {
        var outcome = await Processor(_publisher).ProcessAsync(Message(47m), CancellationToken.None);

        Assert.Equal(new ReadingOutcome(SiteCode, SensorType.ShelterTemperature, 47m, AlarmSeverity.Critical), outcome);
        var alarm = Assert.Single(_publisher.Published);
        Assert.Equal((AlarmSeverity.Critical, 45m), (alarm.Severity, alarm.Threshold.Critical));
    }

    [Fact]
    public async Task Process_NoThresholdForType_ThrowsMissingThreshold()
    {
        var message = Message(1m) with { SensorType = SensorType.TowerTilt };

        var ex = await Assert.ThrowsAsync<MissingAlarmThresholdException>(
            () => Processor(_publisher).ProcessAsync(message, CancellationToken.None));
        Assert.Contains(nameof(SensorType.TowerTilt), ex.Message);
    }

    [Fact]
    public async Task Process_BlankSiteCode_ThrowsValidation()
    {
        await Assert.ThrowsAsync<DomainValidationException>(
            () => Processor(_publisher).ProcessAsync(Message(1m) with { SiteCode = "" }, CancellationToken.None));
    }

    [Fact]
    public async Task LoggingPublisher_WritesWarning()
    {
        var logger = new RecordingLogger<LoggingAlarmPublisher>();
        var alarm = new SensorAlarm(new SensorReading(SiteCode, SensorType.ShelterTemperature, 47m, RecordedAt),
            AlarmSeverity.Critical, Thresholds().Value[SensorType.ShelterTemperature]);

        await new LoggingAlarmPublisher(logger).PublishAsync(alarm, CancellationToken.None);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Contains(SiteCode, entry.Message);
    }

    [Fact]
    public void AddIngestion_BindsThresholdsFromConfiguration()
    {
        var provider = BuildProvider(new Dictionary<string, string?>
        {
            ["AlarmThresholds:BatteryVoltage:Warning"] = "48",
            ["AlarmThresholds:BatteryVoltage:Critical"] = "46",
            ["AlarmThresholds:BatteryVoltage:Direction"] = "LowIsBad"
        });

        var thresholds = provider.GetRequiredService<IOptions<AlarmThresholdOptions>>().Value;

        Assert.Equal(new AlarmThreshold(48m, 46m, ThresholdDirection.LowIsBad), thresholds[SensorType.BatteryVoltage]);
        Assert.IsType<SensorReadingProcessor>(provider.GetRequiredService<ISensorReadingProcessor>());
    }

    private static SensorReadingMessage Message(decimal value) =>
        new(SiteCode, SensorType.ShelterTemperature, value, RecordedAt);

    private static ServiceProvider BuildProvider(Dictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return new ServiceCollection().AddLogging().AddIngestion(configuration).BuildServiceProvider();
    }
}
