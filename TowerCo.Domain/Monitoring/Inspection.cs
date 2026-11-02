using TowerCo.Domain.Common;
using TowerCo.Domain.Entities;

namespace TowerCo.Domain.Monitoring;

/// <summary>Structural inspection carried out on a site's tower.</summary>
public sealed class Inspection : Entity
{
    private Inspection() { }

    public Inspection(Site site, DateOnly inspectedOn, string inspector, InspectionOutcome outcome, string? notes)
    {
        Guard.AgainstBlank(inspector, nameof(inspector));
        Site = site;
        InspectedOn = inspectedOn;
        Inspector = inspector;
        Outcome = outcome;
        Notes = notes;
    }

    public int SiteId { get; private set; }
    public Site Site { get; private set; } = null!;
    public DateOnly InspectedOn { get; private set; }
    public string Inspector { get; private set; } = string.Empty;
    public InspectionOutcome Outcome { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>Whole months since the inspection, as of the given date.</summary>
    public int MonthsSince(DateOnly asOf) =>
        (asOf.Year - InspectedOn.Year) * 12 + asOf.Month - InspectedOn.Month - (asOf.Day < InspectedOn.Day ? 1 : 0);
}
