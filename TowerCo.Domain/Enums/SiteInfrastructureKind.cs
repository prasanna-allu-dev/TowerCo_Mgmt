namespace TowerCo.Domain.Enums;

/// <summary>Kinds of TowerCo-owned site infrastructure (not leased, no capacity impact).</summary>
public enum SiteInfrastructureKind
{
    Generator,
    Battery,
    Solar,
    Cooling
}
