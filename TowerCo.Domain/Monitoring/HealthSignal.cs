namespace TowerCo.Domain.Monitoring;

/// <summary>One scored input to tower health, with a human-readable reason.</summary>
public sealed record HealthSignal(string Name, HealthStatus Status, string Reason)
{
    /// <summary>Overall health is the worst signal; no signals means healthy.</summary>
    public static HealthStatus Worst(IEnumerable<HealthSignal> signals) =>
        signals.Select(s => s.Status).DefaultIfEmpty(HealthStatus.Healthy).Max();

    public static HealthStatus FromAlarm(AlarmSeverity severity) => severity switch
    {
        AlarmSeverity.Critical => HealthStatus.Critical,
        AlarmSeverity.Warning => HealthStatus.Warning,
        _ => HealthStatus.Healthy
    };
}
