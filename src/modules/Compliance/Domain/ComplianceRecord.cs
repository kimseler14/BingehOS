using FacilityOS.Shared;

namespace FacilityOS.Modules.Compliance.Domain;

public class ComplianceRecord : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending";
    public DateTime DueDate { get; private set; }

    public static ComplianceRecord Create(Guid tenantId, string title, string description, string status, DateTime dueDate)
        => new() { TenantId = tenantId, Title = title, Description = description, Status = status, DueDate = dueDate };

    public void Update(string title, string description, string status, DateTime dueDate)
    {
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
    }
}
