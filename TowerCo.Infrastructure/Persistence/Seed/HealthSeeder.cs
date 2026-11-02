using Microsoft.EntityFrameworkCore;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Infrastructure.Persistence.Seed;

/// <summary>
/// Simulated telemetry (one reading per sensor per day for the last 30 days, Active sites only) and inspection history.
/// A few sites are planted with drifting sensors so the health page shows warning and critical towers.
/// </summary>
internal static class HealthSeeder
{
    private const int TelemetryDays = 30;
    private static readonly string[] Inspectors = ["Ravi Shankar", "Lerato Dlamini", "Otieno Mwangi", "Ngozi Adeyemi"];
    private static readonly int[] InspectionMonthsAgo = [3, 14, 8, 27, 5, 11, 2, 18, 6, 9, 30, 4];

    private static readonly Dictionary<SensorType, decimal> Baseline = new()
    {
        [SensorType.BatteryVoltage] = 52m, [SensorType.ShelterTemperature] = 28m,
        [SensorType.GeneratorFuelLevel] = 70m, [SensorType.TowerTilt] = 0.1m
    };

    /// <summary>Active-site index to (sensor, value reached on the last day).</summary>
    private static readonly Dictionary<int, (SensorType Sensor, decimal Final)> Drift = new()
    {
        [1] = (SensorType.BatteryVoltage, 45.5m), [3] = (SensorType.ShelterTemperature, 38m),
        [5] = (SensorType.GeneratorFuelLevel, 25m), [7] = (SensorType.TowerTilt, 0.6m)
    };

    public static async Task SeedAsync(TowerCoDbContext context)
    {
        if (await context.Inspections.AnyAsync())
        {
            return;
        }
        var sites = await context.Sites.OrderBy(s => s.Id).ToListAsync();
        var today = DateTime.UtcNow.Date; // seed relative to first run so the 30-day window is always populated
        context.Inspections.AddRange(sites.Select((s, i) => NewInspection(s, i, DateOnly.FromDateTime(today))));
        var active = sites.Where(s => s.Status == SiteStatus.Active).ToList();
        context.SensorReadings.AddRange(active.SelectMany((s, i) => Readings(s.SiteCode, i, today)));
        await context.SaveChangesAsync();
    }

    private static Inspection NewInspection(Site site, int index, DateOnly today)
    {
        var outcome = index == 6 ? InspectionOutcome.Fail : index % 3 == 0 ? InspectionOutcome.PassWithActions : InspectionOutcome.Pass;
        return new Inspection(site, today.AddMonths(-InspectionMonthsAgo[index % InspectionMonthsAgo.Length]),
            Inspectors[index % Inspectors.Length], outcome, outcome == InspectionOutcome.Pass ? null : "Follow-up actions logged.");
    }

    private static IEnumerable<SensorReading> Readings(string siteCode, int index, DateTime today) =>
        from day in Enumerable.Range(0, TelemetryDays)
        from sensor in Baseline.Keys
        select new SensorReading(siteCode, sensor, ValueFor(sensor, index, day), today.AddDays(day - TelemetryDays + 1).AddHours(6));

    private static decimal ValueFor(SensorType sensor, int index, int day)
    {
        var start = Baseline[sensor];
        var noise = (decimal)Math.Sin(day + index) * start * 0.01m;
        if (!Drift.TryGetValue(index, out var drift) || drift.Sensor != sensor)
            return Math.Round(start + noise, 2);
        var progress = (decimal)day / (TelemetryDays - 1);
        return Math.Round(start + (drift.Final - start) * progress, 2);
    }
}
