namespace TowerCo.Domain.Monitoring;

/// <summary>Whether a reading becomes a problem when it rises (temperature) or falls (battery voltage).</summary>
public enum ThresholdDirection
{
    HighIsBad,
    LowIsBad
}
