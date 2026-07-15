using BingehOS.Infrastructure;
using BingehOS.Modules.Compliance.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Compliance.Application;

public record CalibrationRecordListItem(
    Guid Id,
    Guid AssetId,
    DateTimeOffset CalibratedAt,
    DateTimeOffset? NextDueAt,
    string Result);

public record GetCalibrationRecordQuery(Guid Id) : IRequest<CalibrationRecordListItem?>;

public record GetCalibrationRecordsQuery(
    int Skip = 0,
    int Take = 20,
    Guid? AssetId = null) : IRequest<IReadOnlyList<CalibrationRecordListItem>>;

public class CreateCalibrationRecordHandler
    : IRequestHandler<CreateCalibrationRecordCommand, Guid>
{
    private readonly AppDbContext _db;

    public CreateCalibrationRecordHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(
        CreateCalibrationRecordCommand cmd,
        CancellationToken ct)
    {
        var record = CalibrationRecord.Create(
            _db.CurrentTenantId,
            cmd.AssetId,
            cmd.CalibratedAt,
            cmd.NextDueAt,
            cmd.Result);

        _db.Set<CalibrationRecord>().Add(record);
        await _db.SaveChangesAsync(ct);
        return record.Id;
    }
}

public class UpdateCalibrationRecordHandler
    : IRequestHandler<UpdateCalibrationRecordCommand, CalibrationRecordDto>
{
    private readonly AppDbContext _db;

    public UpdateCalibrationRecordHandler(AppDbContext db) => _db = db;

    public async Task<CalibrationRecordDto> Handle(
        UpdateCalibrationRecordCommand cmd,
        CancellationToken ct)
    {
        var record = await _db.Set<CalibrationRecord>()
            .FirstOrDefaultAsync(
                e => e.Id == cmd.Id &&
                     e.TenantId == _db.CurrentTenantId &&
                     !e.IsDeleted,
                ct)
            ?? throw new KeyNotFoundException(
                $"CalibrationRecord {cmd.Id} not found.");

        record.Update(
            cmd.AssetId,
            cmd.CalibratedAt,
            cmd.NextDueAt,
            cmd.Result);
        await _db.SaveChangesAsync(ct);

        return new CalibrationRecordDto(
            record.Id,
            record.AssetId,
            record.CalibratedAt,
            record.NextDueAt,
            record.Result);
    }
}

public class GetCalibrationRecordHandler
    : IRequestHandler<GetCalibrationRecordQuery, CalibrationRecordListItem?>
{
    private readonly AppDbContext _db;

    public GetCalibrationRecordHandler(AppDbContext db) => _db = db;

    public async Task<CalibrationRecordListItem?> Handle(
        GetCalibrationRecordQuery q,
        CancellationToken ct)
    {
        return await _db.Set<CalibrationRecord>()
            .Where(e => e.Id == q.Id &&
                        e.TenantId == _db.CurrentTenantId &&
                        !e.IsDeleted)
            .Select(e => new CalibrationRecordListItem(
                e.Id,
                e.AssetId,
                e.CalibratedAt,
                e.NextDueAt,
                e.Result))
            .FirstOrDefaultAsync(ct);
    }
}

public class GetCalibrationRecordsHandler
    : IRequestHandler<GetCalibrationRecordsQuery, IReadOnlyList<CalibrationRecordListItem>>
{
    private readonly AppDbContext _db;

    public GetCalibrationRecordsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<CalibrationRecordListItem>> Handle(
        GetCalibrationRecordsQuery q,
        CancellationToken ct)
    {
        var query = _db.Set<CalibrationRecord>()
            .Where(e => e.TenantId == _db.CurrentTenantId && !e.IsDeleted);

        if (q.AssetId.HasValue)
        {
            query = query.Where(e => e.AssetId == q.AssetId.Value);
        }

        return await query
            .OrderByDescending(e => e.CalibratedAt)
            .Skip(Math.Max(0, q.Skip))
            .Take(Math.Clamp(q.Take, 1, 100))
            .Select(e => new CalibrationRecordListItem(
                e.Id,
                e.AssetId,
                e.CalibratedAt,
                e.NextDueAt,
                e.Result))
            .ToListAsync(ct);
    }
}
