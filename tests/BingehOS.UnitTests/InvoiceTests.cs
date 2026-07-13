using BingehOS.Modules.Finance.Domain;

namespace BingehOS.UnitTests;

public class InvoiceTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var vendor = Guid.NewGuid();
        var invoice = Invoice.Create(tenant, "INV-001", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(30), vendor, 150000, "TRY", "Draft", "Purchase");
        Assert.Equal("INV-001", invoice.InvoiceNumber);
        Assert.Equal(150000, invoice.TotalAmountMinor);
        Assert.Equal("TRY", invoice.Currency);
        Assert.Equal("Draft", invoice.Status);
        Assert.Equal("Purchase", invoice.Type);
    }
}
