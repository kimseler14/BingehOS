namespace BingehOS.Infrastructure;

public interface ITenantProvider
{
    Guid CurrentTenantId { get; }
}
