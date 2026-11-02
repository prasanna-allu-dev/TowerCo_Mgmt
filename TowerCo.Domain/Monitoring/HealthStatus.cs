namespace TowerCo.Domain.Monitoring;

/// <summary>Tower health level, ordered from best to worst so the overall status is the maximum.</summary>
public enum HealthStatus
{
    Healthy,
    Warning,
    Critical
}
