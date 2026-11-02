using TowerCo.Domain.Common;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Domain.Entities;

/// <summary>Physical location TowerCo operates. Aggregate root for its tower.</summary>
public sealed class Site : Entity
{
    private Site() { }

    public Site(string siteCode, string name, SiteLocation location, SiteStatus status, DateTime createdAtUtc)
    {
        Guard.AgainstBlank(siteCode, nameof(siteCode));
        Guard.AgainstBlank(name, nameof(name));
        SiteCode = siteCode;
        Name = name;
        Location = location;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public string SiteCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public SiteLocation Location { get; private set; } = null!;
    public SiteStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Tower? Tower { get; private set; }

    public Tower AddTower(TowerType towerType, decimal heightMetres, int maxTenants)
    {
        if (Tower is not null)
        {
            throw new TowerAlreadyExistsException(SiteCode);
        }

        Tower = new Tower(this, towerType, heightMetres, maxTenants);
        return Tower;
    }

    /// <summary>
    /// Moves the site along its lifecycle: Planned, UnderConstruction, Active, Decommissioned (no going back).
    /// Activation needs a tower; decommissioning needs every tenancy ended on or before the given date.
    /// </summary>
    public void ChangeStatus(SiteStatus target, DateOnly asOf)
    {
        if (!AllowedTransitions[Status].Contains(target))
        {
            throw new InvalidSiteTransitionException(SiteCode, Status, target);
        }
        if (target == SiteStatus.Active && Tower is null)
        {
            throw new SiteHasNoTowerException(SiteCode);
        }
        if (target == SiteStatus.Decommissioned)
        {
            EnsureNoCommittedTenants(asOf);
        }
        Status = target;
    }

    public void Rename(string name)
    {
        Guard.AgainstBlank(name, nameof(name));
        Name = name.Trim();
    }

    private void EnsureNoCommittedTenants(DateOnly asOf)
    {
        var committed = Tower?.Tenancies.Count(t => t.IsCommittedFrom(asOf)) ?? 0;
        if (committed > 0)
        {
            throw new SiteHasActiveTenantsException(SiteCode, committed);
        }
    }

    private static readonly Dictionary<SiteStatus, SiteStatus[]> AllowedTransitions = new()
    {
        [SiteStatus.Planned] = [SiteStatus.UnderConstruction, SiteStatus.Decommissioned],
        [SiteStatus.UnderConstruction] = [SiteStatus.Active, SiteStatus.Decommissioned],
        [SiteStatus.Active] = [SiteStatus.Decommissioned],
        [SiteStatus.Decommissioned] = []
    };
}
