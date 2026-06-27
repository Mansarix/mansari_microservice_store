using System.Text.Json;
using Mansari.Store.Contracts.Abstractions;

namespace Mansari.Store.Catalog.Infrastructure.Persistence.Outbox;

public static class OutboxMessageFactory
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static OutboxMessage Create(IIntegrationEvent integrationEvent)
    {
        return new OutboxMessage
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().AssemblyQualifiedName!,
            Content = JsonSerializer.Serialize(
                integrationEvent,
                integrationEvent.GetType(),
                JsonSerializerOptions),
            OccurredOnUtc = integrationEvent.OccurredOnUtc,
            RetryCount = 0
        };
    }
}
