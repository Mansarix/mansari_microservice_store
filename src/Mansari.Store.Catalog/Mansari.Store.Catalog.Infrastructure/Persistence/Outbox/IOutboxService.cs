using Mansari.Store.Contracts.Abstractions;

namespace Mansari.Store.Catalog.Infrastructure.Persistence.Outbox;

public interface IOutboxService
{
    void Add(IIntegrationEvent integrationEvent);
}
