namespace TicketTracker.Api.Contracts.Auth;

public class LoginResponse
{
    public required string AccessToken { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}
