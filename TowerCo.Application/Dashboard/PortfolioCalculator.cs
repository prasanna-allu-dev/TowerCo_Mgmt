using TowerCo.Application.Common;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Dashboard;

/// <summary>Pure calculations for each dashboard section.</summary>
internal static class PortfolioCalculator
{
    public static PortfolioKpisDto Kpis(PortfolioSnapshot snapshot)
    {
        var towers = snapshot.Towers;
        var totalSlots = towers.Sum(t => t.Capacity.MaxTenants);
        var occupiedSlots = towers.Sum(t => t.Capacity.ActiveTenantCount);
        return new(towers.Count, towers.Count(t => t.IsActiveSite), totalSlots, occupiedSlots,
            Percentage.Of(occupiedSlots, totalSlots), towers.Count(t => !t.Capacity.HasCapacity),
            towers.Count(t => t.Capacity.ActiveTenantCount == 0));
    }

    public static IReadOnlyDictionary<string, int> SitesByStatus(PortfolioSnapshot snapshot) =>
        Enum.GetValues<SiteStatus>().ToDictionary(s => s.ToString(), s => snapshot.Towers.Count(t => t.Site.Status == s));

    public static IReadOnlyList<CountryUtilisationDto> UtilisationByCountry(PortfolioSnapshot snapshot) => snapshot.Towers
        .GroupBy(t => t.Site.Location.CountryCode)
        .Select(ToCountryUtilisation)
        .OrderByDescending(c => c.UtilisationPercent)
        .ToList();

    public static IReadOnlyList<CurrencyAmountDto> RentByCurrency(PortfolioSnapshot snapshot) => snapshot.ActiveTenancies
        .GroupBy(t => t.MonthlyRent.Currency)
        .Select(g => new CurrencyAmountDto(g.Key, g.Sum(t => t.MonthlyRent.Amount)))
        .ToList();

    public static IReadOnlyList<OperatorShareDto> OperatorShares(PortfolioSnapshot snapshot)
    {
        var active = snapshot.ActiveTenancies.ToList();
        return snapshot.Operators
            .Select(o => new OperatorShareDto(o.Id, o.Name, active.Count(t => t.OperatorId == o.Id)))
            .OrderByDescending(o => o.ActiveTenancies)
            .ToList();
    }

    public static IReadOnlyList<ExpiringTenancyDto> ExpiringTenancies(PortfolioSnapshot snapshot, int windowDays) => snapshot.Towers
        .SelectMany(t => t.Tower.Tenancies.Where(x => x.IsActiveOn(snapshot.Today) && x.EndDate is not null)
            .Select(x => new ExpiringTenancyDto(x.Id, t.Site.Id, t.Site.SiteCode, x.Operator.Name, x.EndDate!.Value,
                x.EndDate.Value.DayNumber - snapshot.Today.DayNumber)))
        .Where(e => e.DaysRemaining <= windowDays)
        .OrderBy(e => e.DaysRemaining)
        .ToList();

    public static IReadOnlyList<SellableTowerDto> SellableTowers(PortfolioSnapshot snapshot) => snapshot.Towers
        .Where(t => t.IsActiveSite && t.Capacity.HasCapacity)
        .Select(t => new SellableTowerDto(t.Site.Id, t.Site.SiteCode, t.Site.Name, t.Site.Location.CountryCode, t.Capacity.AvailableSlots))
        .OrderByDescending(t => t.AvailableSlots)
        .ToList();

    private static CountryUtilisationDto ToCountryUtilisation(IGrouping<string, TowerSnapshot> country)
    {
        var totalSlots = country.Sum(t => t.Capacity.MaxTenants);
        var occupiedSlots = country.Sum(t => t.Capacity.ActiveTenantCount);
        return new(country.Key, country.Count(), occupiedSlots, totalSlots, Percentage.Of(occupiedSlots, totalSlots));
    }
}
