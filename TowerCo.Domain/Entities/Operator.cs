using TowerCo.Domain.Common;

namespace TowerCo.Domain.Entities;

/// <summary>Mobile network operator that leases tower space.</summary>
public sealed class Operator : Entity
{
    private Operator() { }

    public Operator(string name, string countryCode)
    {
        Guard.AgainstBlank(name, nameof(name));
        Guard.AgainstBlank(countryCode, nameof(countryCode));
        Name = name;
        CountryCode = countryCode;
    }

    public string Name { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
}
