using Mansari.Store.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Mansari.Store.Catalog.Infrastructure.Persistence.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        //builder.ToTable("books");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Navigation(x => x.Title).IsRequired();
        builder.Navigation(x => x.Author).IsRequired();
        builder.Navigation(x => x.Price).IsRequired();
        builder.Navigation(x => x.Stock).IsRequired();
    }
}
