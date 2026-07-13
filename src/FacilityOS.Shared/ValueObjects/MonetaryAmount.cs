namespace FacilityOS.Shared.ValueObjects;

public readonly record struct MonetaryAmount(long MinorAmount, string Currency)
{
    public static MonetaryAmount FromDecimal(decimal amount, string currency)
        => new MonetaryAmount((long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero), currency);

    public decimal ToDecimal() => MinorAmount / 100m;

    public static MonetaryAmount operator +(MonetaryAmount a, MonetaryAmount b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException($"Currency mismatch: {a.Currency} vs {b.Currency}");
        return new MonetaryAmount(a.MinorAmount + b.MinorAmount, a.Currency);
    }
}
