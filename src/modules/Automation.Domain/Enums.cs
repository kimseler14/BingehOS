namespace BingehOS.Modules.Automation.Domain;

public enum AutomationTriggerType
{
    WorkOrderStatusChanged,
    WorkOrderCreated,
    InventoryStockLow,
    CalibrationDue
}

public enum AutomationActionType
{
    CreateWorkOrder,
    SendNotification,
    AdjustPriority
}
