using System.ComponentModel.DataAnnotations;

namespace TowerCo.Application.Dashboard;

/// <summary>Bound from the "Dashboard" section of appsettings.json and validated on start.</summary>
public sealed class DashboardOptions
{
    /// <summary>Tenancies ending within this many days are listed as renewals due.</summary>
    [Range(1, int.MaxValue)]
    public int RenewalWindowDays { get; init; }

    /// <summary>Renewals ending within this many days are raised as critical alerts.</summary>
    [Range(0, int.MaxValue)]
    public int RenewalCriticalDays { get; init; }
}
