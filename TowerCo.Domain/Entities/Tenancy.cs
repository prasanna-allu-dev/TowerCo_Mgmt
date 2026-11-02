using TowerCo.Domain.Common;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Domain.Entities;

/// <summary>Commercial lease of one slot on a tower by an operator.</summary>
public sealed class Tenancy : Entity
{
    private Tenancy() { }

    internal Tenancy(Tower tower, Operator mobileOperator, DateOnly startDate, DateOnly? endDate, Money monthlyRent)
    {
        Guard.AgainstEndBeforeStart(startDate, endDate);
        Guard.AgainstNonPositive(monthlyRent.Amount, nameof(monthlyRent));
        Tower = tower;
        Operator = mobileOperator;
        StartDate = startDate;
        EndDate = endDate;
        MonthlyRent = monthlyRent;
    }

    public int TowerId { get; private set; }
    public Tower Tower { get; private set; } = null!;
    public int OperatorId { get; private set; }
    public Operator Operator { get; private set; } = null!;
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public Money MonthlyRent { get; private set; } = null!;

    /// <summary>Active (holds a slot) when it has no end date or ends on or after the given date.</summary>
    public bool IsActiveOn(DateOnly date) => EndDate is null || EndDate.Value >= date;

    /// <summary>Matches by reference for unsaved entities, by key once persisted.</summary>
    public bool IsHeldBy(Operator mobileOperator) =>
        ReferenceEquals(Operator, mobileOperator) || (mobileOperator.Id != 0 && OperatorId == mobileOperator.Id);
}
