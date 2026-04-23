using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CapstoneProjectRegistration.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneProjectRegistration.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>Demo login: email admin@fpt.edu.vn, password Admin@123 (hardcoded for demonstration).</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || request.Password is null)
        {
            return BadRequest("Email and password are required.");
        }

        const string demoEmail = "admin@fpt.edu.vn";
        const string demoPassword = "Admin@123";

        if (!string.Equals(request.Email, demoEmail, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(request.Password, demoPassword, StringComparison.Ordinal))
        {
            return Unauthorized("Invalid email or password.");
        }

        await HttpContext.Session.LoadAsync(cancellationToken);
        HttpContext.Session.SetString("UserEmail", request.Email.Trim());

        var secret = _configuration["Jwt:SecretKey"] ?? string.Empty;
        if (string.IsNullOrEmpty(secret) || secret.Length < 32)
        {
            return StatusCode(500, "Server JWT configuration is invalid.");
        }

        var issuer = _configuration["Jwt:Issuer"] ?? "CapstoneProjectRegistration";
        var audience = _configuration["Jwt:Audience"] ?? "CapstoneProjectRegistration.Clients";
        var expirySeconds = 2 * 60 * 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, request.Email.Trim()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddSeconds(expirySeconds),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new LoginResponseDto
        {
            Token = tokenString,
            TokenType = "Bearer",
            ExpiresInSeconds = expirySeconds,
            Message = "Login successful. Use Authorization: Bearer {token} for protected routes."
        });
    }
}
