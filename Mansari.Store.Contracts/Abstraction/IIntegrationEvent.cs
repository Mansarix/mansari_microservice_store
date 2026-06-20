namespace Mansari.Store.Contracts.Abstractions;

public interface IIntegrationEvent
{
    Guid Id { get; }
    Guid CorrelationId { get; }
    DateTime OccurredOnUtc { get; }
}
