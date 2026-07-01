namespace TicketTracker.Api.Contracts.Epics;

public class EpicResponse
{
    public required Guid Id { get; set; }
    public required Guid TeamId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}
