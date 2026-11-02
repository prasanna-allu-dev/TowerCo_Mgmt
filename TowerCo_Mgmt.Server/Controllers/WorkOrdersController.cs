using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.WorkOrders;

namespace TowerCo_Mgmt.Server.Controllers;

/// <summary>Work order management. Invalid status moves return 409; assigning without an engineer returns 400.</summary>
// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize(Policy = "Operations")]: field work is raised and progressed by operations users.
[ApiController]
[Route(ApiRoutes.WorkOrders)]
public sealed class WorkOrdersController(IWorkOrderService workOrders) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<WorkOrderDto>> List([FromQuery] WorkOrderFilter filter, CancellationToken ct) =>
        workOrders.ListAsync(filter, ct);

    [HttpGet(ApiRoutes.ById)]
    public Task<WorkOrderDto> Get(int id, CancellationToken ct) => workOrders.GetAsync(id, ct);

    [HttpPost]
    [ProducesResponseType<WorkOrderDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateWorkOrderRequest request, CancellationToken ct)
    {
        var order = await workOrders.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpPatch(ApiRoutes.WorkOrderStatus)]
    public Task<WorkOrderDto> UpdateStatus(int id, UpdateWorkOrderStatusRequest request, CancellationToken ct) =>
        workOrders.UpdateStatusAsync(id, request, ct);
}
