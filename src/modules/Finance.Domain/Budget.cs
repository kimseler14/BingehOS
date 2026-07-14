using BingehOS.Shared;

namespace BingehOS.Modules.Finance.Domain;

public class Budget : BaseEntity
{
    public Guid CostCenterId { get; private set; }
    public BudgetType Type { get; private set; }
    public int Year { get; private set; }
    public long PlannedAmountMinor { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string? Notes { get; private set; }

    public static Budget Create(Guid tenantId, Guid costCenterId, BudgetType type, int year, long plannedAmountMinor, string currency, string? notes)
        => new()
        {
            TenantId = tenantId,
            CostCenterId = costCenterId,
            Type = type,
            Year = year,
            PlannedAmountMinor = plannedAmountMinor,
            Currency = currency,
            Notes = notes
        };
}
