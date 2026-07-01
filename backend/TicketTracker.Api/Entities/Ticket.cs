using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TicketType Type { get; set; }
    public TicketState State { get; set; }

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid? EpicId { get; set; }
    public Epic? Epic { get; set; }

    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public Guid? AssigneeUserId { get; set; }
    public User? AssigneeUser { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
