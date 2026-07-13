using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class CostCenter : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid? ParentCostCenterId { get; private set; }
    public long BudgetMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public bool IsActive { get; private set; } = true;

    public static CostCenter Create(Guid tenantId, string code, string name, Guid? parentCostCenterId, long budgetMinor, string currency, bool isActive)
        => new() { TenantId = tenantId, Code = code, Name = name, ParentCostCenterId = parentCostCenterId, BudgetMinor = budgetMinor, Currency = currency, IsActive = isActive };
}
