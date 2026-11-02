using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Constants;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.Sites;

/// <summary>Moves a site along its lifecycle (Planned, UnderConstruction, Active, Decommissioned).</summary>
public sealed class ChangeSiteStatusRequest
{
    public required SiteStatus Status { get; init; }
}

public sealed class RenameSiteRequest
{
    [Required, StringLength(ValidationRules.NameMaxLength)]
    public required string Name { get; init; }
}
