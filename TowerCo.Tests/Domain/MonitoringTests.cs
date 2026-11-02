using TowerCo.Domain.Exceptions;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Tests.Domain;

public class MonitoringTests
{
    private static readonly AlarmThreshold Temperature = new(35m, 45m, ThresholdDirection.HighIsBad);
    private static readonly AlarmThreshold Battery = new(48m, 46m, ThresholdDirection.LowIsBad);

    [Theory]
    [InlineData(34.9, AlarmSeverity.None)]
    [InlineData(35.0, AlarmSeverity.Warning)]
    [InlineData(44.9, AlarmSeverity.Warning)]
    [InlineData(45.0, AlarmSeverity.Critical)]
    public void Evaluate_HighIsBad_GradesAtBoundaries(decimal value, AlarmSeverity expected)
    {
        Assert.Equal(expected, Temperature.Evaluate(value));
    }

    [Theory]
    [InlineData(48.1, AlarmSeverity.None)]
    [InlineData(48.0, AlarmSeverity.Warning)]
    [InlineData(46.1, AlarmSeverity.Warning)]
    [InlineData(46.0, AlarmSeverity.Critical)]
    public void Evaluate_LowIsBad_GradesAtBoundaries(decimal value, AlarmSeverity expected)
    {
        Assert.Equal(expected, Battery.Evaluate(value));
    }

    [Fact]
    public void SensorReading_KeepsValues()
    {
        var at = new DateTime(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);

        var reading = new SensorReading("IN-MUM-0001", SensorType.TowerTilt, 0.2m, at);

        Assert.Equal(("IN-MUM-0001", SensorType.TowerTilt, 0.2m, at),
            (reading.SiteCode, reading.SensorType, reading.Value, reading.RecordedAtUtc));
    }

    [Fact]
    public void SensorReading_BlankSiteCode_ThrowsValidation()
    {
        Assert.Throws<DomainValidationException>(
            () => new SensorReading(" ", SensorType.TowerTilt, 0.2m, DateTime.UtcNow));
    }
}
