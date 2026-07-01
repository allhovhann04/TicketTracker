namespace TicketTracker.Api.Entities;

public class Team
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Epic> Epics { get; set; } = new List<Epic>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
