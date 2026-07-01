using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Contracts.Auth;

public class ResendVerificationRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
