using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Data.Configurations;

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired();

        builder.HasIndex(t => t.Token)
            .IsUnique();

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.HasOne(t => t.User)
            .WithMany(u => u.EmailVerificationTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
