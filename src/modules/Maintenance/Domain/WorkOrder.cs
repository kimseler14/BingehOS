using FacilityOS.Shared;

namespace FacilityOS.Modules.Maintenance.Domain;

public enum WorkOrderStatus
{
    Draft, Requested, Approved, Rejected, Assigned,
    InProgress, OnHold, Completed, Verified, Closed
}

public class WorkOrder : BaseEntity
{
    public Guid AssetId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public WorkOrderStatus Status { get; private set; } = WorkOrderStatus.Draft;
    public bool PermitApproved { get; private set; }
    public bool ESignatureCaptured { get; private set; }

    public static WorkOrder Create(Guid tenantId, Guid assetId, string description)
        => new() { TenantId = tenantId, AssetId = assetId, Description = description };

    public void ApprovePermit() => PermitApproved = true;
    public void CaptureESignature() => ESignatureCaptured = true;

    private static readonly HashSet<(WorkOrderStatus, WorkOrderStatus)> _allowed = new()
    {
        (WorkOrderStatus.Draft, WorkOrderStatus.Requested),
        (WorkOrderStatus.Requested, WorkOrderStatus.Approved),
        (WorkOrderStatus.Requested, WorkOrderStatus.Rejected),
        (WorkOrderStatus.Approved, WorkOrderStatus.Assigned),
        (WorkOrderStatus.Assigned, WorkOrderStatus.InProgress),
        (WorkOrderStatus.InProgress, WorkOrderStatus.OnHold),
        (WorkOrderStatus.OnHold, WorkOrderStatus.InProgress),
        (WorkOrderStatus.InProgress, WorkOrderStatus.Completed),
        (WorkOrderStatus.Completed, WorkOrderStatus.Verified),
        (WorkOrderStatus.Verified, WorkOrderStatus.Closed),
    };

    public void TransitionTo(WorkOrderStatus next)
    {
        if (!_allowed.Contains((Status, next)))
            throw new InvalidOperationException($"Illegal transition {Status} -> {next}");

        if (next == WorkOrderStatus.InProgress && !PermitApproved)
            throw new InvalidOperationException("Permit to Work must be approved before IN_PROGRESS.");
        if (next == WorkOrderStatus.Closed && !ESignatureCaptured)
            throw new InvalidOperationException("Legal e-signature required to CLOSE.");

        Status = next;
    }
}
