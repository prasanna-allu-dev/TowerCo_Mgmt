namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when money in different currencies is combined.</summary>
public sealed class CurrencyMismatchException(string expected, string actual)
    : BusinessRuleException($"Cannot combine {expected} with {actual}.");
