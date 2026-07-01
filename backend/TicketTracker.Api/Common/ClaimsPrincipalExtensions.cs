using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TicketTracker.Api.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("The current principal does not have a subject claim.");

        return Guid.Parse(subject);
    }
}
