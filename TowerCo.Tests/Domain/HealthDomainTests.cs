using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.Monitoring;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Tests.Domain;

public class HealthDomainTests
{
    private static readonly Site Site = new("IN-MUM-0001", "Andheri", new SiteLocation("IN", "Mumbai", 19, 72),
        SiteStatus.Active, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    [Fact]
    public void Worst_NoSignals_IsHealthy() => Assert.Equal(HealthStatus.Healthy, HealthSignal.Worst([]));

    [Fact]
    public void Worst_ReturnsMostSevere()
    {
        HealthSignal[] signals = [new("a", HealthStatus.Warning, ""), new("b", HealthStatus.Critical, ""), new("c", HealthStatus.Healthy, "")];

        Assert.Equal(HealthStatus.Critical, HealthSignal.Worst(signals));
    }

    [Theory]
    [InlineData(AlarmSeverity.None, HealthStatus.Healthy)]
    [InlineData(AlarmSeverity.Warning, HealthStatus.Warning)]
    [InlineData(AlarmSeverity.Critical, HealthStatus.Critical)]
    public void FromAlarm_MapsSeverity(AlarmSeverity severity, HealthStatus expected) =>
        Assert.Equal(expected, HealthSignal.FromAlarm(severity));

    [Theory]
    [InlineData("2025-11-02", 12)]
    [InlineData("2025-11-03", 11)]
    [InlineData("2026-11-02", 0)]
    public void MonthsSince_CountsWholeMonths(string inspectedOn, int expected)
    {
        var inspection = new Inspection(Site, DateOnly.Parse(inspectedOn), "Inspector", InspectionOutcome.Pass, null);

        Assert.Equal(expected, inspection.MonthsSince(new DateOnly(2026, 11, 2)));
    }

    [Fact]
    public void Inspection_BlankInspector_Throws() =>
        Assert.Throws<DomainValidationException>(() => new Inspection(Site, new DateOnly(2026, 1, 1), " ", InspectionOutcome.Fail, null));
}
