using CapstoneProjectRegistration.Repositories.Entities;

namespace CapstoneProjectRegistration.API.Security;

public interface IJwtTokenService
{
    (string Token, int ExpiresInSeconds) CreateAccessToken(ApplicationUser user, IList<string> roles);
}
