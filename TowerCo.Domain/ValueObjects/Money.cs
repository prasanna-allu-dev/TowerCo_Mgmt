namespace TowerCo.Domain.ValueObjects;

/// <summary>Amount in a given ISO 4217 currency.</summary>
public sealed record Money(decimal Amount, string Currency);
