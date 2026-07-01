using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketTracker.Api.Data.Conversions;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion(new TicketTypeConverter())
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(t => t.State)
            .HasConversion(new TicketStateConverter())
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(t => t.UpdatedAt)
            .HasDefaultValueSql("now()");

        // Team is required. Cross-team epic/ticket consistency is enforced in the service layer, not the database.
        builder.HasOne(t => t.Team)
            .WithMany(team => team.Tickets)
            .HasForeignKey(t => t.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Epic is optional.
        builder.HasOne(t => t.Epic)
            .WithMany(e => e.Tickets)
            .HasForeignKey(t => t.EpicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.CreatedByUser)
            .WithMany(u => u.CreatedTickets)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.AssigneeUser)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssigneeUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
