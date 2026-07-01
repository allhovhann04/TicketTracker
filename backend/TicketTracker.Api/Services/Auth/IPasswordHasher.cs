namespace TicketTracker.Api.Services.Auth;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string encodedHash);
}
