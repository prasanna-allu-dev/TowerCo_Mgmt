using TowerCo.Domain.Common;

namespace TowerCo.Application.Common;

public static class Percentage
{
    /// <summary>Part of total as a percentage, rounded like tower utilisation. Zero when total is zero.</summary>
    public static decimal Of(int part, int total) => total == 0
        ? 0m
        : Math.Round(part * DomainConstants.PercentMultiplier / (decimal)total, DomainConstants.UtilisationDecimals);
}
