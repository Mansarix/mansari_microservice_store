using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mansari.Store.Catalog.Infrastructure.Persistence.Inbox;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        //builder.ToTable("InboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ReceivedOnUtc)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(4000);

        builder.HasIndex(x => x.ProcessedOnUtc);
    }
}

