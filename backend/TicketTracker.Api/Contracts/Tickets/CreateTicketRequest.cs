using System.ComponentModel.DataAnnotations;
using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Contracts.Tickets;

public class CreateTicketRequest
{
    [Required]
    public required Guid TeamId { get; set; }

    public Guid? EpicId { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Body { get; set; }

    [Required]
    public required TicketType Type { get; set; }
}
