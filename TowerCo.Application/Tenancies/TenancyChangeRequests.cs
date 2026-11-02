using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Constants;

namespace TowerCo.Application.Tenancies;

/// <summary>Notice to end a tenancy on a date; the slot is free from the next day.</summary>
public sealed class EndTenancyRequest
{
    public required DateOnly EndDate { get; init; }
}

/// <summary>Extends a fixed-term tenancy. A null end date makes it open ended.</summary>
public sealed class RenewTenancyRequest
{
    public DateOnly? EndDate { get; init; }
}

/// <summary>New monthly rent in the tenancy's existing currency.</summary>
public sealed class RentReviewRequest
{
    [Range(ValidationRules.MinMonthlyRent, double.MaxValue)]
    public required decimal MonthlyRent { get; init; }
}
