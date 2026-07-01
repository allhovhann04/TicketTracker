using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Contracts.Tickets;

public class TicketResponse
{
    public required Guid Id { get; set; }
    public required Guid TeamId { get; set; }
    public Guid? EpicId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required TicketType Type { get; set; }
    public required TicketState State { get; set; }
    public required Guid CreatedByUserId { get; set; }
    public Guid? AssigneeUserId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}
