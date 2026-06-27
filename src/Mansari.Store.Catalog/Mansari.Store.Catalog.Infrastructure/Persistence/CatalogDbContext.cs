using Mansari.Store.Catalog.Domain.Entities;
using Mansari.Store.Catalog.Infrastructure.Persistence.Inbox;
using Mansari.Store.Catalog.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Mansari.Store.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    //inbox and outbox pattern
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

    }
}
