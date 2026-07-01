using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // citext gives case-insensitive equality/uniqueness natively in PostgreSQL.
        builder.Property(u => u.Email)
            .HasColumnType("citext")
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}
