namespace TowerCo.Domain.Exceptions;

/// <summary>Root of all expected business failures. The API maps subtypes to HTTP status codes.</summary>
public abstract class DomainException(string message) : Exception(message);
