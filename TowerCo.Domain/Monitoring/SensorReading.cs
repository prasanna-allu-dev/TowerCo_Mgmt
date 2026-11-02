using TowerCo.Domain.Common;

namespace TowerCo.Domain.Monitoring;

/// <summary>One normalised measurement from a tower sensor.</summary>
public sealed record SensorReading
{
    public SensorReading(string siteCode, SensorType sensorType, decimal value, DateTime recordedAtUtc)
    {
        Guard.AgainstBlank(siteCode, nameof(siteCode));
        SiteCode = siteCode;
        SensorType = sensorType;
        Value = value;
        RecordedAtUtc = recordedAtUtc;
    }

    public string SiteCode { get; }
    public SensorType SensorType { get; }
    public decimal Value { get; }
    public DateTime RecordedAtUtc { get; }
}
