using Mansari.Store.Contracts.Abstractions;

namespace Mansari.Store.Contracts.Orders;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    Guid BookId,
    int Quantity,
    Guid CorrelationId
) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
