namespace BingehOS.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task Publish<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : notnull;
}
