using Microsoft.Extensions.Options;
using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;

namespace TowerCo.Application.Dashboard;

public interface IDashboardService
{
    Task<PortfolioDashboardDto> GetAsync(CancellationToken cancellationToken);
}

/// <summary>UC4. Loads the portfolio once and builds every dashboard section from the same snapshot.</summary>
internal sealed class DashboardService(
    ITowerRepository towers, IOperatorRepository operators, TimeProvider timeProvider, IOptions<DashboardOptions> options)
    : IDashboardService
{
    public async Task<PortfolioDashboardDto> GetAsync(CancellationToken cancellationToken)
    {
        var allTowers = await towers.ListAsync(cancellationToken);
        var allOperators = await operators.ListAsync(cancellationToken);
        var snapshot = PortfolioSnapshot.Create(allTowers, allOperators, timeProvider.Today());
        return Build(snapshot, options.Value);
    }

    private static PortfolioDashboardDto Build(PortfolioSnapshot snapshot, DashboardOptions settings)
    {
        var expiring = PortfolioCalculator.ExpiringTenancies(snapshot, settings.RenewalWindowDays);
        return new(PortfolioCalculator.Kpis(snapshot), PortfolioCalculator.SitesByStatus(snapshot),
            PortfolioCalculator.UtilisationByCountry(snapshot), PortfolioCalculator.RentByCurrency(snapshot),
            PortfolioCalculator.OperatorShares(snapshot), expiring, PortfolioCalculator.SellableTowers(snapshot),
            DashboardAlertBuilder.Build(snapshot, expiring, settings.RenewalCriticalDays));
    }
}
