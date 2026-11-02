using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Health;

namespace TowerCo_Mgmt.Server.Controllers;

/// <summary>UC5: tower health per site (telemetry, inspections, open work orders).</summary>
// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize(Policy = "Operations")]: tower health is operational data.
[ApiController]
[Route(ApiRoutes.HealthSites)]
public sealed class HealthController(ITowerHealthQueryService health) : ControllerBase
{
    [HttpGet]
    public Task<PortfolioHealthDto> List([FromQuery] HealthFilter filter, CancellationToken ct) =>
        health.GetPortfolioAsync(filter, ct);

    [HttpGet(ApiRoutes.BySiteId)]
    public Task<SiteHealthDto> Get(int siteId, CancellationToken ct) => health.GetSiteAsync(siteId, ct);
}
