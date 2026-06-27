namespace Mansari.Store.Catalog.Application.Interfaces.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
}
