using TowerCo.Domain.Exceptions;

namespace TowerCo.Domain.Common;

/// <summary>Invariant checks used by entity constructors and methods.</summary>
public static class Guard
{
    public static void AgainstBlank(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{parameterName} is required.");
        }
    }

    public static void AgainstNonPositive(decimal value, string parameterName)
    {
        if (value <= 0)
        {
            throw new DomainValidationException($"{parameterName} must be greater than zero.");
        }
    }

    public static void AgainstEndBeforeStart(DateOnly start, DateOnly? end)
    {
        if (end < start)
        {
            throw new DomainValidationException("End date cannot be before start date.");
        }
    }
}
