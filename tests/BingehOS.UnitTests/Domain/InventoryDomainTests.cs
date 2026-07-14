using BingehOS.Modules.Inventory.Domain;

namespace BingehOS.UnitTests.Domain;

public class InventoryDomainTests
{
    [Fact]
    public void Warehouse_Create_Rename_Deactivate()
    {
        var tenant = Guid.NewGuid();
        var wh = Warehouse.Create(tenant, "Main", "WH-1", "Istanbul", "user-1");

        Assert.Equal(tenant, wh.TenantId);
        Assert.Equal("Main", wh.Name);
        Assert.Equal("WH-1", wh.Code);
        Assert.Equal("Istanbul", wh.Address);
        Assert.Equal("user-1", wh.ManagerUserId);
        Assert.True(wh.IsActive);
        Assert.Empty(wh.Locations);

        wh.Rename("Central");
        Assert.Equal("Central", wh.Name);

        wh.Deactivate();
        Assert.False(wh.IsActive);
    }

    [Fact]
    public void Location_Shelf_Bin_Create_And_Rename()
    {
        var tenant = Guid.NewGuid();
        var whId = Guid.NewGuid();
        var loc = Location.Create(tenant, whId, "Aisle A", "A", "desc");
        Assert.Equal(whId, loc.WarehouseId);
        Assert.Equal("Aisle A", loc.Name);
        Assert.Empty(loc.Shelves);
        loc.Rename("Aisle B");
        Assert.Equal("Aisle B", loc.Name);

        var shelf = Shelf.Create(tenant, loc.Id, "Shelf 1", "S1", "d");
        Assert.Equal(loc.Id, shelf.LocationId);
        Assert.Empty(shelf.Bins);
        shelf.Rename("Shelf 2");
        Assert.Equal("Shelf 2", shelf.Name);

        var bin = Bin.Create(tenant, shelf.Id, "Bin 1", "B1", 100.0, "d");
        Assert.Equal(shelf.Id, bin.ShelfId);
        Assert.Equal(100.0, bin.MaxCapacity);
        bin.Rename("Bin 2");
        Assert.Equal("Bin 2", bin.Name);
    }

    [Fact]
    public void InventoryTransaction_Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var partId = Guid.NewGuid();
        var binId = Guid.NewGuid();
        var woId = Guid.NewGuid();
        var poId = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;
        var tx = InventoryTransaction.Create(tenant, partId, binId, TransactionType.Issue, 5, "pcs", woId, poId, "note");
        var after = DateTimeOffset.UtcNow;

        Assert.Equal(partId, tx.PartId);
        Assert.Equal(binId, tx.BinId);
        Assert.Equal(TransactionType.Issue, tx.Type);
        Assert.Equal(5, tx.Quantity);
        Assert.Equal("pcs", tx.UnitOfMeasure);
        Assert.Equal(woId, tx.RelatedWorkOrderId);
        Assert.Equal(poId, tx.RelatedPurchaseOrderId);
        Assert.Equal("note", tx.Notes);
        Assert.InRange(tx.TransactionDate, before, after);
    }

    [Fact]
    public void Contract_Create_Deactivate_Activate()
    {
        var tenant = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var start = DateTimeOffset.UtcNow;
        var end = start.AddYears(1);
        var c = Contract.Create(tenant, vendorId, "C-1", "Service", start, end, "terms", "http://doc");

        Assert.Equal(vendorId, c.VendorId);
        Assert.Equal("C-1", c.ContractNumber);
        Assert.Equal("Service", c.Title);
        Assert.True(c.IsActive);

        c.Deactivate();
        Assert.False(c.IsActive);
        c.Activate();
        Assert.True(c.IsActive);
    }

    [Fact]
    public void PurchaseRequest_Lifecycle()
    {
        var tenant = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pr = PurchaseRequest.Create(tenant, "PR-1", userId, "need parts");

        Assert.Equal("PR-1", pr.RequestNumber);
        Assert.Equal(userId, pr.RequestedByUserId);
        Assert.Equal(PurchaseRequestStatus.Draft, pr.Status);
        Assert.Empty(pr.PurchaseOrders);

        pr.Submit();
        Assert.Equal(PurchaseRequestStatus.Submitted, pr.Status);

        var approver = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;
        pr.Approve(approver);
        var after = DateTimeOffset.UtcNow;
        Assert.Equal(PurchaseRequestStatus.Approved, pr.Status);
        Assert.Equal(approver, pr.ApprovedByUserId);
        Assert.NotNull(pr.ApprovedAt);
        Assert.InRange(pr.ApprovedAt.Value, before, after);
    }

    [Fact]
    public void PurchaseRequest_Reject_Sets_Status()
    {
        var pr = PurchaseRequest.Create(Guid.NewGuid(), "PR-2", Guid.NewGuid(), null);
        pr.Reject();
        Assert.Equal(PurchaseRequestStatus.Rejected, pr.Status);
    }

    [Fact]
    public void PurchaseOrder_Lifecycle()
    {
        var tenant = Guid.NewGuid();
        var prId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var expected = DateTimeOffset.UtcNow.AddDays(7);
        var po = PurchaseOrder.Create(tenant, "PO-1", prId, vendorId, expected);

        Assert.Equal("PO-1", po.OrderNumber);
        Assert.Equal(prId, po.PurchaseRequestId);
        Assert.Equal(vendorId, po.VendorId);
        Assert.Equal(PurchaseOrderStatus.Draft, po.Status);
        Assert.Equal(expected, po.ExpectedDeliveryDate);

        po.Send();
        Assert.Equal(PurchaseOrderStatus.Sent, po.Status);
        Assert.NotNull(po.SentAt);

        po.MarkReceived();
        Assert.Equal(PurchaseOrderStatus.Received, po.Status);
        Assert.NotNull(po.ReceivedAt);
    }

    [Fact]
    public void PurchaseOrder_Cancel_Sets_Status()
    {
        var po = PurchaseOrder.Create(Guid.NewGuid(), "PO-2", Guid.NewGuid(), Guid.NewGuid(), null);
        Assert.Null(po.ExpectedDeliveryDate);
        po.Cancel();
        Assert.Equal(PurchaseOrderStatus.Cancelled, po.Status);
    }
}
