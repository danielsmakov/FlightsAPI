using Microsoft.AspNetCore.Identity;

namespace FlightsAPI.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(IdentityUser user);
    }
}
