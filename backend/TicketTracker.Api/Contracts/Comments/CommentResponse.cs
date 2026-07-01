namespace TicketTracker.Api.Contracts.Comments;

public class CommentResponse
{
    public required Guid Id { get; set; }
    public required Guid TicketId { get; set; }
    public required Guid AuthorUserId { get; set; }
    public required string Body { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
