using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Services.Auth;

public interface IJwtTokenGenerator
{
    (string Token, DateTimeOffset ExpiresAt) GenerateToken(User user);
}
