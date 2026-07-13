using BingehOS.Shared;

namespace BingehOS.Modules.HSE.Domain;

public class LotoProcedure : BaseEntity
{
    public string Steps { get; private set; } = "[]";
    public bool IsVerified { get; private set; }
    public Guid? VerifiedBy { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public Guid PermitToWorkId { get; private set; }

    public static LotoProcedure Create(Guid tenantId, string steps, Guid permitToWorkId)
        => new() { TenantId = tenantId, Steps = steps, PermitToWorkId = permitToWorkId };

    public void Verify(Guid verifiedBy)
    {
        IsVerified = true;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTimeOffset.UtcNow;
    }

    public void AddStep(string step)
    {
        var stepsList = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<string>>(Steps) ?? new();
        stepsList.Add(step);
        Steps = System.Text.Json.JsonSerializer.Serialize(stepsList);
    }
}
