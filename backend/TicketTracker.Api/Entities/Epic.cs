namespace TicketTracker.Api.Entities;

public class Epic
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
