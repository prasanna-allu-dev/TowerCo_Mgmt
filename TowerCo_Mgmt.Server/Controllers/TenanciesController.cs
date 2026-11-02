using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Sites;
using TowerCo.Application.Tenancies;
using TowerCo.Application.Tenants;

namespace TowerCo_Mgmt.Server.Controllers;

/// <summary>UC2: tenancy register and adding an operator as tenant (rules live on the Tower aggregate).</summary>
// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize] for reads; [Authorize(Policy = "Commercial")] on Add, End, Renew and ReviewRent (leasing decisions).
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

    /// <summary>Gives notice. Ending before the start, or extending through this call, returns 409.</summary>
    [HttpPost(ApiRoutes.TenancyEnd)]
    public Task<TenancyDto> End(int towerId, int tenancyId, EndTenancyRequest request, CancellationToken ct) =>
        tenancies.EndAsync(towerId, tenancyId, request, ct);

    /// <summary>Extends a fixed term. Returns 409 if open ended, not later, or the extra period has no free slot.</summary>
    [HttpPost(ApiRoutes.TenancyRenewal)]
    public Task<TenancyDto> Renew(int towerId, int tenancyId, RenewTenancyRequest request, CancellationToken ct) =>
        tenancies.RenewAsync(towerId, tenancyId, request, ct);

    [HttpPost(ApiRoutes.TenancyRentReview)]
    public Task<TenancyDto> ReviewRent(int towerId, int tenancyId, RentReviewRequest request, CancellationToken ct) =>
        tenancies.ReviewRentAsync(towerId, tenancyId, request, ct);
}
