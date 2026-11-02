namespace TowerCo.Domain.Common;

/// <summary>Base type for all persisted domain entities.</summary>
public abstract class Entity
{
    public int Id { get; protected set; }
}
