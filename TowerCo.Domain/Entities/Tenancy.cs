using TowerCo.Domain.Common;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Domain.Entities;

/// <summary>
/// Commercial lease of one slot on a tower by an operator, for a period [StartDate, EndDate] (open ended when EndDate is null).
/// Changes go through the Tower aggregate so capacity is re-checked.
/// </summary>
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

    public bool IsOpenEnded => EndDate is null;

    /// <summary>Holds a slot on the date: started on or before it and not yet ended.</summary>
    public bool IsActiveOn(DateOnly date) => StartDate <= date && (EndDate is null || EndDate.Value >= date);

    /// <summary>Active now or starting later: the slot is committed from this date onward.</summary>
    public bool IsCommittedFrom(DateOnly date) => EndDate is null || EndDate.Value >= date;

    /// <summary>True when this tenancy's period shares at least one day with [start, end].</summary>
    public bool Overlaps(DateOnly start, DateOnly? end) =>
        (end is null || StartDate <= end.Value) && (EndDate is null || EndDate.Value >= start);

    /// <summary>Days until the fixed term ends; null for open-ended tenancies, negative once ended.</summary>
    public int? DaysToExpiry(DateOnly asOf) => EndDate is null ? null : EndDate.Value.DayNumber - asOf.DayNumber;

    public bool IsExpiringWithin(DateOnly asOf, int days) => DaysToExpiry(asOf) is { } left && left >= 0 && left <= days;

    /// <summary>Contracted rent for twelve months at the current monthly rate.</summary>
    public Money AnnualValue => MonthlyRent.Annualised();

    /// <summary>Matches by reference for unsaved entities, by key once persisted.</summary>
    public bool IsHeldBy(Operator mobileOperator) =>
        ReferenceEquals(Operator, mobileOperator) || (mobileOperator.Id != 0 && OperatorId == mobileOperator.Id);

    internal void End(DateOnly endDate)
    {
        if (endDate < StartDate)
            throw new InvalidTenancyChangeException("A tenancy cannot end before it starts.");
        if (EndDate is not null && endDate > EndDate)
            throw new InvalidTenancyChangeException("Use renewal to extend a fixed-term tenancy.");
        EndDate = endDate;
    }

    internal void ExtendTo(DateOnly? newEndDate)
    {
        if (EndDate is null)
            throw new InvalidTenancyChangeException("An open-ended tenancy has nothing to renew.");
        if (newEndDate is not null && newEndDate <= EndDate)
            throw new InvalidTenancyChangeException("A renewal must end after the current end date.");
        EndDate = newEndDate;
    }

    internal void ReviewRent(Money newRent)
    {
        MonthlyRent.EnsureSameCurrency(newRent);
        Guard.AgainstNonPositive(newRent.Amount, nameof(newRent));
        MonthlyRent = newRent;
    }
}
