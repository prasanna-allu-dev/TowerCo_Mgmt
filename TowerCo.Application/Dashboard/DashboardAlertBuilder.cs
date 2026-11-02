using TowerCo.Application.Constants;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Dashboard;

/// <summary>Turns capacity and renewal signals into alerts, most severe first.</summary>
internal static class DashboardAlertBuilder
{
    public static IReadOnlyList<DashboardAlertDto> Build(
        PortfolioSnapshot snapshot, IReadOnlyList<ExpiringTenancyDto> expiring, int criticalDays) =>
        CapacityAlerts(snapshot)
            .Concat(expiring.Select(e => RenewalAlert(e, criticalDays)))
            .OrderBy(a => a.Severity)
            .ToList();

    private static IEnumerable<DashboardAlertDto> CapacityAlerts(PortfolioSnapshot snapshot)
    {
        var active = snapshot.Towers.Where(t => t.IsActiveSite).ToList();
        var stranded = snapshot.Towers.Where(t => t.Site.Status == SiteStatus.Decommissioned && t.Capacity.ActiveTenantCount > 0);
        return active.Where(t => !t.Capacity.HasCapacity).Select(FullAlert)
            .Concat(active.Where(t => t.Capacity.ActiveTenantCount == 0).Select(EmptyAlert))
            .Concat(stranded.Select(DecommissionedAlert));
    }

    private static DashboardAlertDto FullAlert(TowerSnapshot t) => new(
        string.Format(AlertMessages.FullId, t.Site.Id), AlertSeverity.Critical,
        string.Format(AlertMessages.FullTitle, t.Site.SiteCode),
        string.Format(AlertMessages.FullDetail, t.Capacity.ActiveTenantCount, t.Capacity.MaxTenants), t.Site.Id);

    private static DashboardAlertDto EmptyAlert(TowerSnapshot t) => new(
        string.Format(AlertMessages.EmptyId, t.Site.Id), AlertSeverity.Warning,
        string.Format(AlertMessages.EmptyTitle, t.Site.SiteCode),
        string.Format(AlertMessages.EmptyDetail, t.Site.Name), t.Site.Id);

    private static DashboardAlertDto DecommissionedAlert(TowerSnapshot t) => new(
        string.Format(AlertMessages.DecommissionedId, t.Site.Id), AlertSeverity.Critical,
        string.Format(AlertMessages.DecommissionedTitle, t.Site.SiteCode),
        string.Format(AlertMessages.DecommissionedDetail, t.Capacity.ActiveTenantCount), t.Site.Id);

    private static DashboardAlertDto RenewalAlert(ExpiringTenancyDto e, int criticalDays) => new(
        string.Format(AlertMessages.RenewalId, e.TenancyId),
        e.DaysRemaining <= criticalDays ? AlertSeverity.Critical : AlertSeverity.Warning,
        string.Format(AlertMessages.RenewalTitle, e.OperatorName, e.SiteCode),
        string.Format(AlertMessages.RenewalDetail, e.EndDate, e.DaysRemaining), e.SiteId);
}
