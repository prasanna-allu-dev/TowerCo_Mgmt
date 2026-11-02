namespace TowerCo.Domain.Monitoring;

/// <summary>Kinds of telemetry a tower site gateway reports. Thresholds per type live in configuration.</summary>
public enum SensorType
{
    BatteryVoltage,
    ShelterTemperature,
    GeneratorFuelLevel,
    TowerTilt
}
