using BingehOS.Modules.Maintenance.Domain;
using Xunit;

namespace BingehOS.UnitTests;

public class WorkOrderStateMachineTests
{
    [Fact]
    public void NewWorkOrder_IsDraft()
    {
        var wo = WorkOrder.Create(Guid.NewGuid(), Guid.NewGuid(), "Fix HVAC");
        Assert.Equal(WorkOrderStatus.Draft, wo.Status);
    }

    [Fact]
    public void Assigned_To_InProgress_RequiresPermit()
    {
        var wo = WorkOrder.Create(Guid.NewGuid(), Guid.NewGuid(), "Fix HVAC");
        wo.TransitionTo(WorkOrderStatus.Requested);
        wo.TransitionTo(WorkOrderStatus.Approved);
        wo.TransitionTo(WorkOrderStatus.Assigned);
        Assert.Throws<InvalidOperationException>(() =>
            wo.TransitionTo(WorkOrderStatus.InProgress)); // no permit approved
    }

    [Fact]
    public void Verified_To_Closed_RequiresSignature()
    {
        var wo = WorkOrder.Create(Guid.NewGuid(), Guid.NewGuid(), "Fix HVAC");
        wo.TransitionTo(WorkOrderStatus.Requested);
        wo.TransitionTo(WorkOrderStatus.Approved);
        wo.TransitionTo(WorkOrderStatus.Assigned);
        wo.ApprovePermit();
        wo.TransitionTo(WorkOrderStatus.InProgress);
        wo.TransitionTo(WorkOrderStatus.Completed);
        wo.TransitionTo(WorkOrderStatus.Verified);
        Assert.Throws<InvalidOperationException>(() =>
            wo.TransitionTo(WorkOrderStatus.Closed)); // no e-signature
    }

    [Fact]
    public void IllegalTransition_Throws()
    {
        var wo = WorkOrder.Create(Guid.NewGuid(), Guid.NewGuid(), "Fix HVAC");
        // Draft cannot jump directly to Approved.
        Assert.Throws<InvalidOperationException>(() =>
            wo.TransitionTo(WorkOrderStatus.Approved));
    }

    [Fact]
    public void FullHappyPath_WithPermitAndSignature_ReachesClosed()
    {
        var wo = WorkOrder.Create(Guid.NewGuid(), Guid.NewGuid(), "Fix HVAC");
        wo.TransitionTo(WorkOrderStatus.Requested);
        wo.TransitionTo(WorkOrderStatus.Approved);
        wo.TransitionTo(WorkOrderStatus.Assigned);
        wo.ApprovePermit();
        wo.TransitionTo(WorkOrderStatus.InProgress);
        wo.TransitionTo(WorkOrderStatus.Completed);
        wo.TransitionTo(WorkOrderStatus.Verified);
        wo.CaptureESignature();
        wo.TransitionTo(WorkOrderStatus.Closed);
        Assert.Equal(WorkOrderStatus.Closed, wo.Status);
    }
}
