using CapstoneProjectRegistration.API.Models;
using CapstoneProjectRegistration.API.Security;
using CapstoneProjectRegistration.Repositories.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers;

[ApiController]
[Tags("01 · Authentication")]
[Route("api/v1/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>Issue a JWT for a registered account (seeded dev users; password in Swagger description).</summary>
    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<IActionResult> IssueToken([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || request.Password is null)
        {
            return BadRequest("Email and password are required.");
        }

        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresIn) = _jwtTokenService.CreateAccessToken(user, roles);

        return Ok(new LoginResponseDto
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresInSeconds = expiresIn,
            Message = $"Authenticated as {string.Join(", ", roles)}. Use Authorization: Bearer {{token}}."
        });
    }
}
