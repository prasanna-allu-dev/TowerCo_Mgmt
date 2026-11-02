using TowerCo.Application.Sites;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Dashboard;

/// <summary>One tower with its capacity derived once for the dashboard date.</summary>
internal sealed record TowerSnapshot(Tower Tower, CapacitySummaryDto Capacity)
{
    public Site Site => Tower.Site;
    public bool IsActiveSite => Site.Status == SiteStatus.Active;
}

/// <summary>Portfolio state as of one date. All dashboard sections are calculated from this.</summary>
internal sealed record PortfolioSnapshot(IReadOnlyList<TowerSnapshot> Towers, IReadOnlyList<Operator> Operators, DateOnly Today)
{
    public static PortfolioSnapshot Create(IReadOnlyList<Tower> towers, IReadOnlyList<Operator> operators, DateOnly today) =>
        new(towers.Select(t => new TowerSnapshot(t, SiteMapper.ToCapacity(t, today))).ToList(), operators, today);

    public IEnumerable<Tenancy> ActiveTenancies => Towers.SelectMany(t => t.Tower.Tenancies).Where(t => t.IsActiveOn(Today));
}
