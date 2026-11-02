namespace TowerCo.Domain.ValueObjects;

/// <summary>Where a site physically is. CountryCode is ISO 3166-1 alpha-2.</summary>
public sealed record SiteLocation(string CountryCode, string City, double Latitude, double Longitude);
