using TowerCo.Domain.Common;
using TowerCo.Domain.Enums;

namespace TowerCo.Domain.Entities;

/// <summary>Catalogue entry for equipment. New types are data, not code.</summary>
public sealed class EquipmentType : Entity
{
    private EquipmentType() { }

    public EquipmentType(string name, EquipmentCategory category)
    {
        Guard.AgainstBlank(name, nameof(name));
        Name = name;
        Category = category;
    }

    public string Name { get; private set; } = string.Empty;
    public EquipmentCategory Category { get; private set; }
}
