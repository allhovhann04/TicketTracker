namespace TicketTracker.Api.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public Guid AuthorUserId { get; set; }
    public User Author { get; set; } = null!;

    public required string Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
