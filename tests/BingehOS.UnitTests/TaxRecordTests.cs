using BingehOS.Modules.Finance.Domain;

namespace BingehOS.UnitTests;

public class TaxRecordTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var taxRecord = TaxRecord.Create(tenant, invoiceId, "VAT", 0.18m, 27000, "TRY");
        Assert.Equal(invoiceId, taxRecord.InvoiceId);
        Assert.Equal("VAT", taxRecord.TaxType);
        Assert.Equal(0.18m, taxRecord.TaxRate);
        Assert.Equal(27000, taxRecord.TaxAmountMinor);
        Assert.Equal("TRY", taxRecord.Currency);
    }
}
