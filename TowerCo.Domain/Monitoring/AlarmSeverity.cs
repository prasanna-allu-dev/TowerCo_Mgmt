namespace TowerCo.Domain.Monitoring;

/// <summary>Outcome of checking one reading against its threshold. Ordered from least to most severe.</summary>
public enum AlarmSeverity
{
    None,
    Warning,
    Critical
}
