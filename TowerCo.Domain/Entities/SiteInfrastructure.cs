using TowerCo.Domain.Common;
using TowerCo.Domain.Enums;

namespace TowerCo.Domain.Entities;

/// <summary>Power and cooling kit owned and run by TowerCo on a tower. Never leased to tenants; monitored by sensors.</summary>
public sealed class SiteInfrastructure : Entity
{
    private SiteInfrastructure() { }

    internal SiteInfrastructure(Tower tower, SiteInfrastructureKind kind, string serialNumber, DateOnly installedOn)
    {
        Guard.AgainstBlank(serialNumber, nameof(serialNumber));
        Tower = tower;
        Kind = kind;
        SerialNumber = serialNumber;
        InstalledOn = installedOn;
    }

    public int TowerId { get; private set; }
    public Tower Tower { get; private set; } = null!;
    public SiteInfrastructureKind Kind { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public DateOnly InstalledOn { get; private set; }
}
