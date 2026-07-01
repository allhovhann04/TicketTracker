using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        // citext gives case-insensitive equality/uniqueness natively in PostgreSQL.
        builder.Property(t => t.Name)
            .HasColumnType("citext")
            .IsRequired();

        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(t => t.UpdatedAt)
            .HasDefaultValueSql("now()");
    }
}
