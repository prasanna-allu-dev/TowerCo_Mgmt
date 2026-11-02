namespace TowerCo.Infrastructure.Exceptions;

/// <summary>Raised at startup when a required configuration value is absent.</summary>
public sealed class MissingConfigurationException(string key)
    : Exception($"Required configuration '{key}' is missing.");
