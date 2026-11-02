using Microsoft.Extensions.Options;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Health;

/// <summary>Worst latest sensor reading against configured alarm thresholds.</summary>
internal sealed class TelemetryHealthRule(IOptions<SensorThresholdOptions> thresholds) : IHealthSignalRule
{
    public HealthSignal Evaluate(HealthContext context)
    {
        if (context.LatestReadings.Count == 0)
            return new(HealthSignals.Telemetry, HealthStatus.Healthy, HealthSignals.NoTelemetry);

        var scored = context.LatestReadings
            .Select(r => (Reading: r, Status: HealthSignal.FromAlarm(thresholds.Value[r.SensorType].Evaluate(r.Value))))
            .Where(s => s.Status != HealthStatus.Healthy).OrderByDescending(s => s.Status).ToList();
        if (scored.Count == 0)
            return new(HealthSignals.Telemetry, HealthStatus.Healthy, HealthSignals.TelemetryOk);

        var reason = string.Join(", ", scored.Select(s => string.Format(HealthSignals.SensorBreach, s.Reading.SensorType, s.Reading.Value, s.Status)));
        return new(HealthSignals.Telemetry, scored[0].Status, reason);
    }
}

/// <summary>Age and outcome of the most recent structural inspection.</summary>
internal sealed class InspectionHealthRule(IOptions<HealthOptions> options) : IHealthSignalRule
{
    public HealthSignal Evaluate(HealthContext context)
    {
        var last = context.LastInspection;
        if (last is null)
            return new(HealthSignals.Inspection, HealthStatus.Warning, HealthSignals.NoInspection);
        if (last.Outcome == InspectionOutcome.Fail)
            return new(HealthSignals.Inspection, HealthStatus.Critical, string.Format(HealthSignals.InspectionFailed, last.InspectedOn));

        var months = last.MonthsSince(context.Today);
        return new(HealthSignals.Inspection, StatusForAge(months), string.Format(HealthSignals.InspectionAge, months));
    }

    private HealthStatus StatusForAge(int months)
    {
        if (months >= options.Value.InspectionCriticalMonths) return HealthStatus.Critical;
        return months >= options.Value.InspectionWarningMonths ? HealthStatus.Warning : HealthStatus.Healthy;
    }
}

/// <summary>Open work orders: any Critical priority is critical, otherwise any open is a warning.</summary>
internal sealed class WorkOrderHealthRule : IHealthSignalRule
{
    public HealthSignal Evaluate(HealthContext context)
    {
        var open = context.OpenWorkOrders.Count;
        var urgent = context.OpenWorkOrders.Count(w => w.Priority == WorkOrderPriority.Critical);
        if (urgent > 0)
            return new(HealthSignals.WorkOrders, HealthStatus.Critical, string.Format(HealthSignals.UrgentWorkOrders, open, urgent));
        return open > 0
            ? new(HealthSignals.WorkOrders, HealthStatus.Warning, string.Format(HealthSignals.OpenWorkOrders, open))
            : new(HealthSignals.WorkOrders, HealthStatus.Healthy, HealthSignals.NoOpenWorkOrders);
    }
}
