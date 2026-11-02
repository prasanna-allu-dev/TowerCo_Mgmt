using TowerCo.Domain.Common;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Domain.ValueObjects;

/// <summary>Where a site is. Coordinates are WGS84 degrees and must be on the globe.</summary>
public sealed record SiteLocation(string CountryCode, string City, double Latitude, double Longitude)
{
    public string CountryCode { get; } = CountryCode?.Trim().Length == DomainConstants.CountryCodeLength
        ? CountryCode.Trim().ToUpperInvariant()
        : throw new DomainValidationException($"Country code must be {DomainConstants.CountryCodeLength} letters.");

    public string City { get; } = string.IsNullOrWhiteSpace(City) ? throw new DomainValidationException("City is required.") : City.Trim();

    public double Latitude { get; } = Math.Abs(Latitude) <= DomainConstants.MaxLatitude
        ? Latitude : throw new DomainValidationException($"Latitude must be between -{DomainConstants.MaxLatitude} and {DomainConstants.MaxLatitude}.");

    public double Longitude { get; } = Math.Abs(Longitude) <= DomainConstants.MaxLongitude
        ? Longitude : throw new DomainValidationException($"Longitude must be between -{DomainConstants.MaxLongitude} and {DomainConstants.MaxLongitude}.");
}
