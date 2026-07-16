using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Asset.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Asset.Application;

public record MeterListItem(Guid Id, Guid AssetId, string Name, string Unit, string? MeterType, DateTimeOffset? LastReadingAt, double? LastReadingValue);
public record GetMetersQuery(Guid AssetId) : IRequest<IReadOnlyList<MeterListItem>>;
public record RecordMeterReadingCommand(Guid MeterId, double Value) : IRequest;

public class GetMetersHandler : IRequestHandler<GetMetersQuery, IReadOnlyList<MeterListItem>>
{
    private readonly AppDbContext _db;
    public GetMetersHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<MeterListItem>> Handle(GetMetersQuery q, CancellationToken ct)
    {
        return await _db.Set<Meter>()
            .Where(m => m.AssetId == q.AssetId && !m.IsDeleted)
            .Select(m => new MeterListItem(m.Id, m.AssetId, m.Name, m.Unit, m.MeterType, m.LastReadingAt, m.LastReadingValue))
            .ToListAsync(ct);
    }
}

public class RecordMeterReadingHandler : IRequestHandler<RecordMeterReadingCommand>
{
    private readonly AppDbContext _db;
    public RecordMeterReadingHandler(AppDbContext db) => _db = db;

    public async Task Handle(RecordMeterReadingCommand cmd, CancellationToken ct)
    {
        var meter = await _db.Set<Meter>().FirstOrDefaultAsync(m => m.Id == cmd.MeterId && !m.IsDeleted, ct);
        if (meter == null) throw new KeyNotFoundException("Meter not found.");
        meter.RecordReading(cmd.Value);
        await _db.SaveChangesAsync(ct);
    }
}
