using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Constants;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Sites;

/// <summary>Optional filters for the sites list. Text search matches code, name or city.</summary>
public sealed class SiteFilter
{
    [RegularExpression(ValidationRules.CountryCodePattern)]
    public string? Country { get; init; }

    public SiteStatus? Status { get; init; }

    [StringLength(ValidationRules.SearchMaxLength)]
    public string? Search { get; init; }
}
