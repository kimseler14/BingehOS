using BingehOS.Shared;

namespace BingehOS.Modules.HSE.Domain;

public class PermitToWork : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending";
    public Guid? FacilityId { get; private set; }
    public Guid? WorkOrderId { get; private set; }
    public Guid? ApproverId { get; private set; }
    public string? RejectionReason { get; private set; }

    public static PermitToWork Create(Guid tenantId, string title, string description, Guid? facilityId, Guid? workOrderId)
        => new() { TenantId = tenantId, Title = title, Description = description, FacilityId = facilityId, WorkOrderId = workOrderId };

    public void Approve(Guid approverId)
    {
        Status = "Approved";
        ApproverId = approverId;
        RejectionReason = null;
    }

    public void Reject(Guid approverId, string reason)
    {
        Status = "Rejected";
        ApproverId = approverId;
        RejectionReason = reason;
    }
}
