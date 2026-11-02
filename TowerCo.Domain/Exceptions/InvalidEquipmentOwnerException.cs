namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when equipment is installed for a tenancy that is not on the same tower. All equipment is tenant owned.</summary>
public sealed class InvalidEquipmentOwnerException(string siteCode)
    : BusinessRuleException($"Equipment on site '{siteCode}' must belong to a tenancy on the same tower.");
