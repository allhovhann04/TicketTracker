using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Contracts.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

    [Required]
    public required string DisplayName { get; set; }
}
