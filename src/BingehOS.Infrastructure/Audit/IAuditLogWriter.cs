using BingehOS.Modules.Audit.Domain;

namespace BingehOS.Infrastructure.Audit;

public interface IAuditLogWriter
{
    Task WriteAsync(AuditLog log, CancellationToken ct = default);
}