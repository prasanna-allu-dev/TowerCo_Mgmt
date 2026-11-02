using System.Text.RegularExpressions;
using TowerCo.Domain.Common;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Domain.ValueObjects;

/// <summary>Amount in an ISO 4217 currency. Never negative; arithmetic only within one currency.</summary>
public sealed partial record Money(decimal Amount, string Currency)
{
    public decimal Amount { get; } = Amount >= 0 ? Amount : throw new DomainValidationException("Amount cannot be negative.");

    public string Currency { get; } = CurrencyPattern().IsMatch(Currency ?? string.Empty)
        ? Currency!.ToUpperInvariant()
        : throw new DomainValidationException($"Currency must be a {DomainConstants.CurrencyCodeLength}-letter ISO code.");

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Times(int factor) => new(Amount * factor, Currency);

    /// <summary>Twelve months of this monthly amount.</summary>
    public Money Annualised() => Times(DomainConstants.MonthsPerYear);

    public void EnsureSameCurrency(Money other)
    {
        if (other.Currency != Currency)
        {
            throw new CurrencyMismatchException(Currency, other.Currency);
        }
    }

    [GeneratedRegex("^[A-Za-z]{3}$")]
    private static partial Regex CurrencyPattern();
}
