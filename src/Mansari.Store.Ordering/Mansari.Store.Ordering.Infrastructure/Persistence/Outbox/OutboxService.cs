using Mansari.Store.Contracts.Abstractions;
using Mansari.Store.Ordering.Application.Abstractions.Messaging;

namespace Mansari.Store.Ordering.Infrastructure.Persistence.Outbox;

public sealed class OutboxService : IOutboxService
{
    private readonly OrderingDbContext _dbContext;

    public OutboxService(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(IIntegrationEvent integrationEvent)
    {
        var outboxMessage = OutboxMessageFactory.Create(integrationEvent);

        _dbContext.OutboxMessages.Add(outboxMessage);
    }
}
