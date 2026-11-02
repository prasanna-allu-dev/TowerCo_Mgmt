using TowerCo.Domain.Entities;
using TowerCo.Domain.Enums;
using TowerCo.Domain.Exceptions;
using TowerCo.Domain.ValueObjects;

namespace TowerCo.Tests.Domain;

public class WorkOrderTests
{
    private static readonly DateTime Now = new(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);
    private static readonly Site Site = new("IN-MUM-0001", "Andheri", new SiteLocation("IN", "Mumbai", 19, 72), SiteStatus.Active, Now);

    private static WorkOrderDetails Details(string title = "Replace aviation light") =>
        new(title, "Light off", WorkOrderCategory.Corrective, WorkOrderPriority.High, new DateOnly(2026, 11, 20));

    private static WorkOrder NewOrder(string? assignee = null) => new(Site, Details(), assignee, Now);

    [Fact]
    public void Constructor_NoAssignee_IsOpen()
    {
        var order = NewOrder();

        Assert.Equal(WorkOrderStatus.Open, order.Status);
        Assert.Null(order.Assignee);
        Assert.Null(order.CompletedAtUtc);
    }

    [Fact]
    public void Constructor_WithAssignee_IsAssigned()
    {
        Assert.Equal(WorkOrderStatus.Assigned, NewOrder("Priya Nair").Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Fix")]
    public void Constructor_ShortTitle_ThrowsValidation(string title)
    {
        Assert.Throws<InvalidWorkOrderTitleException>(() => new WorkOrder(Site, Details(title), null, Now));
    }

    [Fact]
    public void ChangeStatus_FullHappyPath_SetsCompletedAt()
    {
        var order = NewOrder();

        order.ChangeStatus(WorkOrderStatus.Assigned, "Priya Nair", Now);
        order.ChangeStatus(WorkOrderStatus.InProgress, null, Now);
        order.ChangeStatus(WorkOrderStatus.OnHold, null, Now);
        order.ChangeStatus(WorkOrderStatus.InProgress, null, Now);
        order.ChangeStatus(WorkOrderStatus.Completed, null, Now.AddHours(3));

        Assert.Equal("Priya Nair", order.Assignee);
        Assert.Equal(Now.AddHours(3), order.CompletedAtUtc);
    }

    [Fact]
    public void ChangeStatus_AssignWithoutEngineer_ThrowsValidation()
    {
        Assert.Throws<WorkOrderAssigneeRequiredException>(() => NewOrder().ChangeStatus(WorkOrderStatus.Assigned, " ", Now));
    }

    [Theory]
    [InlineData(WorkOrderStatus.Completed)]
    [InlineData(WorkOrderStatus.InProgress)]
    public void ChangeStatus_DisallowedFromOpen_Throws(WorkOrderStatus target)
    {
        var exception = Assert.Throws<InvalidWorkOrderTransitionException>(() => NewOrder().ChangeStatus(target, null, Now));

        Assert.Contains($"from Open to {target}", exception.Message);
    }

    [Fact]
    public void ChangeStatus_FromCancelled_Throws()
    {
        var order = NewOrder();
        order.ChangeStatus(WorkOrderStatus.Cancelled, null, Now);

        Assert.Throws<InvalidWorkOrderTransitionException>(() => order.ChangeStatus(WorkOrderStatus.Assigned, "A", Now));
    }
}
