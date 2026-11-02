namespace TowerCo.Domain.Monitoring;

/// <summary>Warning and critical limits for one sensor type. Values come from configuration, not code.</summary>
public sealed record AlarmThreshold(decimal Warning, decimal Critical, ThresholdDirection Direction)
{
    public AlarmSeverity Evaluate(decimal value)
    {
        if (IsBreached(value, Critical))
        {
            return AlarmSeverity.Critical;
        }
        return IsBreached(value, Warning) ? AlarmSeverity.Warning : AlarmSeverity.None;
    }

    private bool IsBreached(decimal value, decimal limit) =>
        Direction == ThresholdDirection.HighIsBad ? value >= limit : value <= limit;
}
