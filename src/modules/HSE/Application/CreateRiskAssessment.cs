using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Messaging;
using BingehOS.Modules.HSE.Domain;
using MediatR;

namespace BingehOS.Modules.HSE.Application;

public class CreateRiskAssessmentHandler : IRequestHandler<CreateRiskAssessmentCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _eventPublisher;

    public CreateRiskAssessmentHandler(AppDbContext db, IEventPublisher eventPublisher)
    {
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> Handle(CreateRiskAssessmentCommand cmd, CancellationToken ct)
    {
        var assessment = RiskAssessment.Create(_db.CurrentTenantId, cmd.Title, cmd.Description, cmd.Level, cmd.PermitToWorkId);
        _db.Set<RiskAssessment>().Add(assessment);
        await _db.SaveChangesAsync(ct);
        await _eventPublisher.Publish(new RiskAssessmentCreatedEvent(assessment.Id, assessment.Title, assessment.Level), ct);
        return assessment.Id;
    }
}
