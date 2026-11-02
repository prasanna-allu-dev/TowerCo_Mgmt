using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Constants;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Sites;

/// <summary>UC1 thin slice: registers a site and its tower in one request. New sites start as Planned.</summary>
public sealed class CreateSiteRequest
{
    [RegularExpression(ValidationRules.SiteCodePattern)]
    public required string SiteCode { get; init; }

    [Required, StringLength(ValidationRules.NameMaxLength)]
    public required string Name { get; init; }

    [RegularExpression(ValidationRules.CountryCodePattern)]
    public required string CountryCode { get; init; }

    [Required, StringLength(ValidationRules.NameMaxLength)]
    public required string City { get; init; }

    [Range(-ValidationRules.MaxLatitude, ValidationRules.MaxLatitude)]
    public required double Latitude { get; init; }

    [Range(-ValidationRules.MaxLongitude, ValidationRules.MaxLongitude)]
    public required double Longitude { get; init; }

    public required TowerType TowerType { get; init; }

    [Range(ValidationRules.MinTowerHeightMetres, ValidationRules.MaxTowerHeightMetres)]
    public required decimal HeightMetres { get; init; }

    [Range(ValidationRules.MinTenantsPerTower, ValidationRules.MaxTenantsPerTower)]
    public required int MaxTenants { get; init; }
}
