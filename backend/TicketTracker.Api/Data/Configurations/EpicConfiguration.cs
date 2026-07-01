using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Data.Configurations;

public class EpicConfiguration : IEntityTypeConfiguration<Epic>
{
    public void Configure(EntityTypeBuilder<Epic> builder)
    {
        builder.HasKey(e => e.Id);

        // citext gives case-insensitive equality/uniqueness natively in PostgreSQL.
        builder.Property(e => e.Title)
            .HasColumnType("citext")
            .IsRequired();

        // Not unique: the specification only requires a non-empty trimmed title, not
        // uniqueness. Kept as a plain index since Tickets/ListAsync filters by TeamId.
        builder.HasIndex(e => e.TeamId);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()");

        builder.HasOne(e => e.Team)
            .WithMany(t => t.Epics)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
