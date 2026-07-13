using BingehOS.Infrastructure;
using BingehOS.Modules.Audit.Domain;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Infrastructure.Audit;

public class AuditLogWriter : IAuditLogWriter
{
    private readonly AppDbContext _db;
    public AuditLogWriter(AppDbContext db) => _db = db;

    public async Task WriteAsync(AuditLog log, CancellationToken ct = default)
    {
        _db.Set<AuditLog>().Add(log);
        await _db.SaveChangesAsync(ct);
    }
}