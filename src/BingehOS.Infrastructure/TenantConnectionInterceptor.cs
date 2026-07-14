using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BingehOS.Infrastructure;

public class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly AppDbContext _ctx;
    public TenantConnectionInterceptor(AppDbContext ctx) => _ctx = ctx;

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetTenant(connection);
        base.ConnectionOpened(connection, eventData);
    }

    public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        SetTenant(connection);
        return base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private void SetTenant(DbConnection connection)
    {
        if (_ctx.CurrentTenantId == Guid.Empty)
            return;

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT set_config('app.current_tenant_id', @tenant, false);";
        var param = cmd.CreateParameter();
        param.ParameterName = "tenant";
        param.Value = _ctx.CurrentTenantId.ToString();
        cmd.Parameters.Add(param);
        cmd.ExecuteNonQuery();
    }
}
