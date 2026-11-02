namespace TowerCo.Application.Constants;

/// <summary>Input limits for API requests and filters.</summary>
public static class ValidationRules
{
    public const string CountryCodePattern = "^[A-Za-z]{2}$";
    public const string CurrencyCodePattern = "^[A-Z]{3}$";
    public const int SearchMaxLength = 100;
    public const int MinOperatorId = 1;
    public const double MinMonthlyRent = 0.01;
    public const string SiteCodePattern = @"^[A-Z]{2}-[A-Z]{3}-\d{4}$";
    public const int NameMaxLength = 128;
    public const int DescriptionMaxLength = 1000;
    public const double MaxLatitude = 90;
    public const double MaxLongitude = 180;
    public const double MinTowerHeightMetres = 5;
    public const double MaxTowerHeightMetres = 120;
    public const int MinTenantsPerTower = 1;
    public const int MaxTenantsPerTower = 6;
    public const int MinId = 1;
    public const string TenancyStatePattern = "^(" + FilterValues.TenancyActive + "|" + FilterValues.TenancyEnded + ")$";
    public const string OwnerPattern = "^(" + FilterValues.OwnerTenant + "|" + FilterValues.OwnerShared + ")$";
}
