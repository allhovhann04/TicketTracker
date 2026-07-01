using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Contracts.Teams;

public class UpdateTeamRequest
{
    [Required]
    public required string Name { get; set; }
}
