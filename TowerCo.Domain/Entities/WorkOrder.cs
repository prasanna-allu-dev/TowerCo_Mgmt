using TowerCo.Domain.Common;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;

namespace TowerCo.Domain.Entities;

/// <summary>Field job raised against a site. Status workflow mirrors the client state machine.</summary>
public sealed class WorkOrder : Entity
{
    private static readonly Dictionary<WorkOrderStatus, WorkOrderStatus[]> AllowedTransitions = new()
    {
        [WorkOrderStatus.Open] = [WorkOrderStatus.Assigned, WorkOrderStatus.Cancelled],
        [WorkOrderStatus.Assigned] = [WorkOrderStatus.InProgress, WorkOrderStatus.OnHold, WorkOrderStatus.Cancelled],
        [WorkOrderStatus.InProgress] = [WorkOrderStatus.OnHold, WorkOrderStatus.Completed, WorkOrderStatus.Cancelled],
        [WorkOrderStatus.OnHold] = [WorkOrderStatus.InProgress, WorkOrderStatus.Cancelled],
        [WorkOrderStatus.Completed] = [],
        [WorkOrderStatus.Cancelled] = []
    };

    private WorkOrder() { }

    public WorkOrder(Site site, WorkOrderDetails details, string? assignee, DateTime createdAtUtc)
    {
        GuardTitle(details.Title);
        Site = site;
        Title = details.Title.Trim();
        Description = details.Description;
        Category = details.Category;
        Priority = details.Priority;
        DueDate = details.DueDate;
        CreatedAtUtc = createdAtUtc;
        Assignee = string.IsNullOrWhiteSpace(assignee) ? null : assignee;
        Status = Assignee is null ? WorkOrderStatus.Open : WorkOrderStatus.Assigned;
    }

    public int SiteId { get; private set; }
    public Site Site { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public WorkOrderCategory Category { get; private set; }
    public WorkOrderPriority Priority { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public string? Assignee { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    /// <summary>Moves along the workflow. A null assignee keeps the current one; Assigned needs an assignee.</summary>
    public void ChangeStatus(WorkOrderStatus target, string? assignee, DateTime changedAtUtc)
    {
        if (!AllowedTransitions[Status].Contains(target))
        {
            throw new InvalidWorkOrderTransitionException(Status, target);
        }
        var newAssignee = string.IsNullOrWhiteSpace(assignee) ? Assignee : assignee;
        if (target == WorkOrderStatus.Assigned && newAssignee is null)
        {
            throw new WorkOrderAssigneeRequiredException();
        }
        Status = target;
        Assignee = newAssignee;
        CompletedAtUtc = target == WorkOrderStatus.Completed ? changedAtUtc : null;
    }

    private static void GuardTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < DomainConstants.MinWorkOrderTitleLength)
        {
            throw new InvalidWorkOrderTitleException();
        }
    }
}

/// <summary>Descriptive fields supplied when a work order is raised.</summary>
public sealed record WorkOrderDetails(
    string Title, string Description, WorkOrderCategory Category, WorkOrderPriority Priority, DateOnly DueDate);
