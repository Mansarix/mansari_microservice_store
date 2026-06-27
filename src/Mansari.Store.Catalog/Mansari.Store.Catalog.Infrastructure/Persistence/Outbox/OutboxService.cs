using Mansari.Store.Contracts.Abstractions;

namespace Mansari.Store.Catalog.Infrastructure.Persistence.Outbox;

public sealed class OutboxService : IOutboxService
{
    private readonly CatalogDbContext _dbContext;

    public OutboxService(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(IIntegrationEvent integrationEvent)
    {
        var outboxMessage = OutboxMessageFactory.Create(integrationEvent);

        _dbContext.OutboxMessages.Add(outboxMessage);
    }
}
