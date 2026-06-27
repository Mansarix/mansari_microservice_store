using Mansari.Store.Contracts.Abstractions;

namespace Mansari.Store.Ordering.Application.Abstractions.Messaging;

public interface IOutboxService
{
    void Add(IIntegrationEvent integrationEvent);
}
