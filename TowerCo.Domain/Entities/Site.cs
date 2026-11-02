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
}
