using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CapstoneProjectRegistration.Repositories.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneProjectRegistration.API.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, int ExpiresInSeconds) CreateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var secret = _configuration["Jwt:SecretKey"] ?? string.Empty;
        if (string.IsNullOrEmpty(secret) || secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SecretKey must be set and at least 32 characters.");
        }

        var issuer = _configuration["Jwt:Issuer"] ?? "CapstoneProjectRegistration";
        var audience = _configuration["Jwt:Audience"] ?? "CapstoneProjectRegistration.Clients";
        var expirySeconds = int.TryParse(_configuration["Jwt:AccessTokenLifetimeSeconds"], out var s) ? s : 7200;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        if (!string.IsNullOrEmpty(user.UserName))
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddSeconds(expirySeconds),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expirySeconds);
    }
}
