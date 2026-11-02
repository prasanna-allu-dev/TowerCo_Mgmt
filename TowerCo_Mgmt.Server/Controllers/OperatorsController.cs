using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Operators;

namespace TowerCo_Mgmt.Server.Controllers;

// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize]: any signed-in TowerCo user may read operators.
[ApiController]
[Route(ApiRoutes.Operators)]
public sealed class OperatorsController(IOperatorQueryService operators) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<OperatorDto>> List(CancellationToken ct) => operators.ListAsync(ct);
}
