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

        // Epic title must be unique per team, case-insensitively.
        builder.HasIndex(e => new { e.TeamId, e.Title })
            .IsUnique();

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
