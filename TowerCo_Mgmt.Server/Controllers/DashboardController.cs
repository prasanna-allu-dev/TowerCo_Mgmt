using Microsoft.AspNetCore.Mvc;
using TowerCo.Application.Dashboard;

namespace TowerCo_Mgmt.Server.Controllers;

/// <summary>UC4: portfolio overview for managers.</summary>
// AUTH (out of scope for this demo): this controller must require authentication.
// Intended: [Authorize(Policy = "Manager")]: portfolio KPIs and rent are management data.
[ApiController]
[Route(ApiRoutes.Dashboard)]
public sealed class DashboardController(IDashboardService dashboard) : ControllerBase
{
    [HttpGet]
    public Task<PortfolioDashboardDto> Get(CancellationToken ct) => dashboard.GetAsync(ct);
}
