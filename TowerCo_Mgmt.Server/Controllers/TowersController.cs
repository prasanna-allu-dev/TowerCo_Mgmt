using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Towers;

namespace TowerCo_Mgmt.Server.Controllers;

// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize]: any signed-in TowerCo user may read tower configuration.
[ApiController]
[Route(ApiRoutes.Towers)]
public sealed class TowersController(ITowerQueryService towers) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<TowerConfigurationDto>> List(CancellationToken ct) => towers.ListAsync(ct);
}
