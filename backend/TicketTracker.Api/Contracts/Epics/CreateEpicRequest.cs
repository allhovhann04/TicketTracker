using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Contracts.Epics;

public class CreateEpicRequest
{
    [Required]
    public required Guid TeamId { get; set; }

    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }
}
