using BingehOS.Modules.Inventory.Application;
using BingehOS.Modules.Inventory.Domain;

namespace BingehOS.UnitTests;

public class InventoryTransactionsTests
{
    [Fact]
    public void Receiving_And_Return_Increase_Stock()
    {
        var afterReceive = InventoryStockCalculator.Apply(0, TransactionType.Receiving, 10);
        var afterReturn = InventoryStockCalculator.Apply(afterReceive, TransactionType.Return, 2);

        Assert.Equal(10, afterReceive);
        Assert.Equal(12, afterReturn);
    }

    [Fact]
    public void Issue_Decreases_Stock()
    {
        var stock = InventoryStockCalculator.Apply(10, TransactionType.Issue, 3);

        Assert.Equal(7, stock);
    }

    [Fact]
    public void Issue_Rejected_When_Stock_Would_Be_Negative()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => InventoryStockCalculator.Apply(2, TransactionType.Issue, 3));

        Assert.Equal("Insufficient stock.", ex.Message);
    }
}
