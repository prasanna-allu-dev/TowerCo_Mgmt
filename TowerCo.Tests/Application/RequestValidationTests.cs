using System.ComponentModel.DataAnnotations;
using TowerCo.Application.EquipmentRegister;
using TowerCo.Application.Sites;
using TowerCo.Application.Tenants;
using TowerCo.Application.WorkOrders;
using TowerCo.Domain.Enums;

namespace TowerCo.Tests.Application;

/// <summary>Checks the DataAnnotations rules that [ApiController] turns into HTTP 400 responses.</summary>
public class RequestValidationTests
{
    private static bool IsValid(object model) =>
        Validator.TryValidateObject(model, new ValidationContext(model), [], validateAllProperties: true);

    public static TheoryData<object> InvalidModels => new()
    {
        new TenantFilter { State = "pending" },
        new TenantFilter { OperatorId = 0 },
        new TenantFilter { Country = "IND" },
        new EquipmentFilter { SiteId = 0 },
        new WorkOrderFilter { Search = new string('x', 101) },
        new SiteFilter { Country = "1" },
        new CreateSiteRequest
        {
            SiteCode = "bad", Name = "x", CountryCode = "IN", City = "Pune", Latitude = 91, Longitude = 0,
            TowerType = TowerType.Monopole, HeightMetres = 1, MaxTenants = 9
        }
    };

    [Theory]
    [MemberData(nameof(InvalidModels))]
    public void InvalidInput_FailsValidation(object model) => Assert.False(IsValid(model));

    [Fact]
    public void ValidFilters_PassValidation()
    {
        Assert.True(IsValid(new TenantFilter { State = "active", OperatorId = 1, Country = "in" }));
        Assert.True(IsValid(new EquipmentFilter { SiteId = 1, Country = "KE" }));
    }
}
