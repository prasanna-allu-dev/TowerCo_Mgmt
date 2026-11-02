using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Sites;
using TowerCo.Application.Tenancies;
using TowerCo.Application.Tenants;

namespace TowerCo_Mgmt.Server.Controllers;

/// <summary>UC2: tenancy register and adding an operator as tenant (rules live on the Tower aggregate).</summary>
// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize] for reads; [Authorize(Policy = "Commercial")] on Add (UC2 leasing decision).
[ApiController]
[Route(ApiRoutes.Base)]
public sealed class TenanciesController(ITenantQueryService tenants, ITenancyService tenancies) : ControllerBase
{
    [HttpGet(ApiRoutes.Tenancies)]
    public Task<IReadOnlyList<TenantRecordDto>> List([FromQuery] TenantFilter filter, CancellationToken ct) =>
        tenants.ListAsync(filter, ct);

    [HttpPost(ApiRoutes.TowerTenancies)]
    [ProducesResponseType<TenancyDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Add(int towerId, CreateTenancyRequest request, CancellationToken ct)
    {
        var tenancy = await tenancies.AddAsync(towerId, request, ct);
        return StatusCode(StatusCodes.Status201Created, tenancy);
    }
}
