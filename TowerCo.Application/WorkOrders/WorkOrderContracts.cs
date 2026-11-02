using System.ComponentModel.DataAnnotations;
using TowerCo.Application.Constants;
using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;

namespace TowerCo.Application.WorkOrders;

public sealed record WorkOrderDto(
    int Id, string Code, int SiteId, string SiteCode, string SiteName, string CountryCode,
    string Title, string Description, WorkOrderCategory Category, WorkOrderPriority Priority,
    WorkOrderStatus Status, string? Assignee, DateTime CreatedAtUtc, DateOnly DueDate, DateTime? CompletedAtUtc)
{
    public static WorkOrderDto From(WorkOrder order) => new(
        order.Id, string.Format(FilterValues.WorkOrderCodeFormat, order.Id), order.SiteId, order.Site.SiteCode,
        order.Site.Name, order.Site.Location.CountryCode, order.Title, order.Description, order.Category,
        order.Priority, order.Status, order.Assignee, order.CreatedAtUtc, order.DueDate, order.CompletedAtUtc);
}

public sealed class WorkOrderFilter
{
    [StringLength(ValidationRules.SearchMaxLength)]
    public string? Search { get; init; }
    public WorkOrderStatus? Status { get; init; }
    public WorkOrderPriority? Priority { get; init; }
    [RegularExpression(ValidationRules.CountryCodePattern)]
    public string? Country { get; init; }
}

public sealed class CreateWorkOrderRequest
{
    [Range(ValidationRules.MinId, int.MaxValue)]
    public required int SiteId { get; init; }

    [Required, StringLength(ValidationRules.NameMaxLength)]
    public required string Title { get; init; }

    [StringLength(ValidationRules.DescriptionMaxLength)]
    public string Description { get; init; } = string.Empty;

    public required WorkOrderCategory Category { get; init; }
    public required WorkOrderPriority Priority { get; init; }
    public required DateOnly DueDate { get; init; }

    [StringLength(ValidationRules.NameMaxLength)]
    public string? Assignee { get; init; }
}

public sealed class UpdateWorkOrderStatusRequest
{
    public required WorkOrderStatus Status { get; init; }

    [StringLength(ValidationRules.NameMaxLength)]
    public string? Assignee { get; init; }
}
