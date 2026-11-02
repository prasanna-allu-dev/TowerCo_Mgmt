using TowerCo.Application.Abstractions;
using TowerCo.Application.Common;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Application.WorkOrders;

public interface IWorkOrderService
{
    Task<IReadOnlyList<WorkOrderDto>> ListAsync(WorkOrderFilter filter, CancellationToken cancellationToken);
    Task<WorkOrderDto> GetAsync(int id, CancellationToken cancellationToken);
    Task<WorkOrderDto> CreateAsync(CreateWorkOrderRequest request, CancellationToken cancellationToken);
    Task<WorkOrderDto> UpdateStatusAsync(int id, UpdateWorkOrderStatusRequest request, CancellationToken cancellationToken);
}

/// <summary>Work order management. Workflow rules live on the WorkOrder entity.</summary>
internal sealed class WorkOrderService(
    IWorkOrderRepository workOrders, ISiteRepository sites, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IWorkOrderService
{
    public async Task<IReadOnlyList<WorkOrderDto>> ListAsync(WorkOrderFilter filter, CancellationToken cancellationToken)
    {
        var all = await workOrders.ListAsync(cancellationToken);
        return all.Select(WorkOrderDto.From).Where(dto => Matches(dto, filter))
            .OrderByDescending(dto => dto.CreatedAtUtc).ThenByDescending(dto => dto.Id).ToList();
    }

    public async Task<WorkOrderDto> GetAsync(int id, CancellationToken cancellationToken) =>
        WorkOrderDto.From(await Load(id, cancellationToken));

    public async Task<WorkOrderDto> CreateAsync(CreateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var site = await sites.FindAsync(request.SiteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Site), request.SiteId);
        var details = new WorkOrderDetails(request.Title, request.Description, request.Category, request.Priority, request.DueDate);
        var order = new WorkOrder(site, details, request.Assignee, timeProvider.GetUtcNow().UtcDateTime);
        workOrders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return WorkOrderDto.From(order);
    }

    public async Task<WorkOrderDto> UpdateStatusAsync(int id, UpdateWorkOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var order = await Load(id, cancellationToken);
        order.ChangeStatus(request.Status, request.Assignee, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return WorkOrderDto.From(order);
    }

    private async Task<WorkOrder> Load(int id, CancellationToken cancellationToken) =>
        await workOrders.GetAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(WorkOrder), id);

    private static bool Matches(WorkOrderDto dto, WorkOrderFilter filter) =>
        TextFilter.Matches(filter.Search, dto.Code, dto.Title, dto.SiteCode, dto.SiteName)
        && (filter.Status is null || dto.Status == filter.Status)
        && (filter.Priority is null || dto.Priority == filter.Priority)
        && TextFilter.EqualsCode(filter.Country, dto.CountryCode);
}
