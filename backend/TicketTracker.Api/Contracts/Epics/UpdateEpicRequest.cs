using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Contracts.Epics;

public class UpdateEpicRequest
{
    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }
}
