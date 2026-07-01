using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Contracts.Teams;

public class CreateTeamRequest
{
    [Required]
    public required string Name { get; set; }
}
