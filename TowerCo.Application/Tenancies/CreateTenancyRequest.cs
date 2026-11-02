using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Constants;

namespace TowerCo.Application.Tenancies;

/// <summary>Adds an operator as tenant. Missing required members are rejected during JSON binding (HTTP 400).</summary>
public sealed class CreateTenancyRequest
{
    [Range(ValidationRules.MinOperatorId, int.MaxValue)]
    public required int OperatorId { get; init; }

    public required DateOnly StartDate { get; init; }

    [Range(ValidationRules.MinMonthlyRent, double.MaxValue)]
    public required decimal MonthlyRent { get; init; }

    [RegularExpression(ValidationRules.CurrencyCodePattern)]
    public required string Currency { get; init; }
}
