using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.EquipmentRegister;

namespace TowerCo_Mgmt.Server.Controllers;

// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize]: any signed-in TowerCo user may read the equipment register.
[ApiController]
[Route(ApiRoutes.Base)]
public sealed class EquipmentController(IEquipmentQueryService equipment) : ControllerBase
{
    [HttpGet(ApiRoutes.Equipment)]
    public Task<IReadOnlyList<EquipmentItemDto>> List([FromQuery] EquipmentFilter filter, CancellationToken ct) =>
        equipment.ListAsync(filter, ct);

    [HttpGet(ApiRoutes.EquipmentTypes)]
    public Task<IReadOnlyList<EquipmentTypeSummaryDto>> Types(CancellationToken ct) => equipment.ListTypesAsync(ct);
}
