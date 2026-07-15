using MediatR;

namespace BingehOS.Modules.Compliance.Application;

public record CalibrationRecordDto(
    Guid Id,
    Guid AssetId,
    DateTimeOffset CalibratedAt,
    DateTimeOffset? NextDueAt,
    string Result);

public record CreateCalibrationRecordCommand(
    Guid AssetId,
    DateTimeOffset CalibratedAt,
    DateTimeOffset? NextDueAt,
    string Result) : IRequest<Guid>;

public record UpdateCalibrationRecordCommand(
    Guid Id,
    Guid AssetId,
    DateTimeOffset CalibratedAt,
    DateTimeOffset? NextDueAt,
    string Result) : IRequest<CalibrationRecordDto>;
