using Microsoft.Extensions.Options;
using TowerCo.Application.Health;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Monitoring;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Tests.Application;

public class HealthRulesTests
{
    private static readonly DateTime Now = new(2026, 11, 2, 6, 0, 0, DateTimeKind.Utc);
    private static readonly DateOnly Today = DateOnly.FromDateTime(Now);
    private static readonly Site Site = new("IN-MUM-0001", "Andheri", new SiteLocation("IN", "Mumbai", 19, 72), SiteStatus.Active, Now);

    private static readonly SensorThresholdOptions Thresholds = new()
    {
        [SensorType.BatteryVoltage] = new AlarmThreshold(48, 46, ThresholdDirection.LowIsBad),
        [SensorType.ShelterTemperature] = new AlarmThreshold(35, 45, ThresholdDirection.HighIsBad)
    };

    private static readonly HealthOptions Options = new() { TelemetryWindowDays = 30, InspectionWarningMonths = 12, InspectionCriticalMonths = 24 };

    private static HealthContext Context(SensorReading[]? readings = null, Inspection? inspection = null, WorkOrder[]? orders = null) =>
        new(Site, readings ?? [], inspection, orders ?? [], Today);

    private static SensorReading Reading(SensorType type, decimal value) => new(Site.SiteCode, type, value, Now);

    private static HealthSignal Telemetry(params SensorReading[] readings) =>
        new TelemetryHealthRule(Microsoft.Extensions.Options.Options.Create(Thresholds)).Evaluate(Context(readings));

    [Fact]
    public void Telemetry_NoReadings_IsHealthy() => Assert.Equal(HealthSignals.NoTelemetry, Telemetry().Reason);

    [Fact]
    public void Telemetry_AllWithinLimits_IsHealthy()
    {
        var signal = Telemetry(Reading(SensorType.BatteryVoltage, 52), Reading(SensorType.ShelterTemperature, 30));

        Assert.Equal((HealthStatus.Healthy, HealthSignals.TelemetryOk), (signal.Status, signal.Reason));
    }

    [Fact]
    public void Telemetry_WorstBreachWins_AndListsSensors()
    {
        var signal = Telemetry(Reading(SensorType.BatteryVoltage, 45.5m), Reading(SensorType.ShelterTemperature, 36));

        Assert.Equal(HealthStatus.Critical, signal.Status);
        Assert.StartsWith("BatteryVoltage 45.5 (Critical)", signal.Reason);
        Assert.Contains("ShelterTemperature 36 (Warning)", signal.Reason);
    }

    [Theory]
    [InlineData(11, HealthStatus.Healthy)]
    [InlineData(12, HealthStatus.Warning)]
    [InlineData(24, HealthStatus.Critical)]
    public void Inspection_AgeBoundaries(int monthsAgo, HealthStatus expected)
    {
        var inspection = new Inspection(Site, Today.AddMonths(-monthsAgo), "Inspector", InspectionOutcome.Pass, null);

        Assert.Equal(expected, InspectionRule().Evaluate(Context(inspection: inspection)).Status);
    }

    [Fact]
    public void Inspection_None_IsWarning() =>
        Assert.Equal(HealthStatus.Warning, InspectionRule().Evaluate(Context()).Status);

    [Fact]
    public void Inspection_Failed_IsCritical()
    {
        var inspection = new Inspection(Site, Today, "Inspector", InspectionOutcome.Fail, null);

        Assert.Equal(HealthStatus.Critical, InspectionRule().Evaluate(Context(inspection: inspection)).Status);
    }

    [Theory]
    [InlineData(new WorkOrderPriority[0], HealthStatus.Healthy)]
    [InlineData(new[] { WorkOrderPriority.Medium, WorkOrderPriority.High }, HealthStatus.Warning)]
    [InlineData(new[] { WorkOrderPriority.Low, WorkOrderPriority.Critical }, HealthStatus.Critical)]
    public void WorkOrders_ScoredByOpenCountAndPriority(WorkOrderPriority[] priorities, HealthStatus expected)
    {
        var orders = priorities.Select(p => new WorkOrder(Site,
            new WorkOrderDetails("Routine check", "", WorkOrderCategory.Preventive, p, Today), null, Now)).ToArray();

        Assert.Equal(expected, new WorkOrderHealthRule().Evaluate(Context(orders: orders)).Status);
    }

    private static InspectionHealthRule InspectionRule() => new(Microsoft.Extensions.Options.Options.Create(Options));
}
