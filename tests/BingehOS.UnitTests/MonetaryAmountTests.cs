using BingehOS.Shared.ValueObjects;
using Xunit;

namespace BingehOS.UnitTests;

public class MonetaryAmountTests
{
    [Fact]
    public void FromDecimal_StoresMinorUnits()
    {
        var m = MonetaryAmount.FromDecimal(15.50m, "TRY");
        Assert.Equal(1550L, m.MinorAmount);
        Assert.Equal("TRY", m.Currency);
    }

    [Fact]
    public void Add_CombinesMinorUnits_SameCurrency()
    {
        var a = new MonetaryAmount(1000, "TRY");
        var b = new MonetaryAmount(550, "TRY");
        var sum = a + b;
        Assert.Equal(1550L, sum.MinorAmount);
    }

    [Fact]
    public void DifferentCurrency_Add_Throws()
    {
        var a = new MonetaryAmount(1000, "TRY");
        var b = new MonetaryAmount(550, "USD");
        Assert.Throws<InvalidOperationException>(() => _ = a + b);
    }
}
