namespace FacilityOS.Infrastructure;

public interface ITenantProvider
{
    Guid CurrentTenantId { get; }
}
