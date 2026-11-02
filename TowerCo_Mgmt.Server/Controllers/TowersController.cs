using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Sites;
using TowerCo.Application.Towers;

namespace TowerCo_Mgmt.Server.Controllers;

// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize] for reads; [Authorize(Policy = "Operations")] on ChangeCapacity (structural change).
[ApiController]
[Route(ApiRoutes.Towers)]
public sealed class TowersController(ITowerQueryService towers, ITowerCommandService commands) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<TowerConfigurationDto>> List(CancellationToken ct) => towers.ListAsync(ct);

    /// <summary>Changes slot capacity. Dropping below committed tenancies returns 409.</summary>
    [HttpPatch(ApiRoutes.TowerCapacity)]
    public Task<CapacitySummaryDto> ChangeCapacity(int towerId, ChangeCapacityRequest request, CancellationToken ct) =>
        commands.ChangeCapacityAsync(towerId, request, ct);
}
