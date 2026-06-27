using Mansari.Store.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mansari.Store.Users.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);
        builder.Property(x => x.DeletedAtUtc);
        builder.Property(x => x.BirthDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.Gender).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.OwnsOne(x => x.FirstName, owned =>
        {
            owned.Property(p => p.Value)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(x => x.LastName, owned =>
        {
            owned.Property(p => p.Value)
                .HasColumnName("LastName")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(x => x.NationalCode, owned =>
        {
            owned.Property(p => p.Value)
                .HasColumnName("NationalCode")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.OwnsOne(x => x.MobileNumber, owned =>
        {
            owned.Property(p => p.Value)
                .HasColumnName("MobileNumber")
                .HasMaxLength(16)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Username, owned =>
        {
            owned.Property(p => p.Value)
                .HasColumnName("Username")
                .HasMaxLength(32)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Email, owned =>
        {
            owned.Property(p => p!.Value)
                .HasColumnName("Email")
                .HasMaxLength(254);
        });

        builder.HasIndex("NationalCode").IsUnique();
        builder.HasIndex("MobileNumber").IsUnique();
        builder.HasIndex("Username").IsUnique();
        builder.HasIndex("Email").IsUnique().HasFilter("[Email] IS NOT NULL");
    }
}
