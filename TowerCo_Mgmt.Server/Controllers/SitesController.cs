using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Sites;

namespace TowerCo_Mgmt.Server.Controllers;

/// <summary>UC3 search and site view, UC1 site registration.</summary>
// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize] for reads; [Authorize(Policy = "Operations")] on Create (UC1 registers sites).
[ApiController]
[Route(ApiRoutes.Sites)]
public sealed class SitesController(ISiteQueryService queries, ISiteCommandService commands) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<SiteSummaryDto>> List([FromQuery] SiteFilter filter, CancellationToken ct) =>
        queries.ListAsync(filter, ct);

    [HttpGet(ApiRoutes.ById)]
    public Task<SiteDetailDto> Get(int id, CancellationToken ct) => queries.GetAsync(id, ct);

    [HttpPost]
    [ProducesResponseType<SiteDetailDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateSiteRequest request, CancellationToken ct)
    {
        var site = await commands.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = site.Id }, site);
    }
}
