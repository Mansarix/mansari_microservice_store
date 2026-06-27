using Mansari.Store.Ordering.Domain.Orders.Entities;
using Mansari.Store.Ordering.Infrastructure.Persistence.Inbox;
using Mansari.Store.Ordering.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Mansari.Store.Ordering.Infrastructure.Persistence;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    //inbox & outbox pattern
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
