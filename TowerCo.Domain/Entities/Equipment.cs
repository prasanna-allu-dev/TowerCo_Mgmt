using TowerCo.Domain.Common;
using TowerCo.Domain.Enums;

namespace TowerCo.Domain.Entities;

/// <summary>Physical asset on a tower. Tenant owned when linked to a tenancy, otherwise TowerCo shared.</summary>
public sealed class Equipment : Entity
{
    private Equipment() { }

    internal Equipment(Tower tower, EquipmentType type, string serialNumber, DateOnly installedOn, decimal? mountHeightMetres, Tenancy? tenancy)
    {
        Guard.AgainstBlank(serialNumber, nameof(serialNumber));
        Tower = tower;
        EquipmentType = type;
        SerialNumber = serialNumber;
        InstalledOn = installedOn;
        MountHeightMetres = mountHeightMetres;
        Tenancy = tenancy;
        Status = EquipmentStatus.Installed;
    }

    public int TowerId { get; private set; }
    public Tower Tower { get; private set; } = null!;
    public int EquipmentTypeId { get; private set; }
    public EquipmentType EquipmentType { get; private set; } = null!;
    public int? TenancyId { get; private set; }
    public Tenancy? Tenancy { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public DateOnly InstalledOn { get; private set; }
    public decimal? MountHeightMetres { get; private set; }
    public EquipmentStatus Status { get; private set; }

    public void Remove() => Status = EquipmentStatus.Removed;
}
