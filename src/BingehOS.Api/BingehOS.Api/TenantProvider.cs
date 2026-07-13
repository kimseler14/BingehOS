using BingehOS.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace BingehOS.Api;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _accessor;
    public TenantProvider(IHttpContextAccessor accessor) => _accessor = accessor;

    public Guid CurrentTenantId
    {
        get
        {
            if (_accessor.HttpContext?.Items.TryGetValue("TenantId", out var v) == true &&
                v is Guid guid)
            {
                return guid;
            }
            return Guid.Empty;
        }
    }
}
