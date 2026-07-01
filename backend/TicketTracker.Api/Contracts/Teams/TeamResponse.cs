namespace TicketTracker.Api.Contracts.Teams;

public class TeamResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
