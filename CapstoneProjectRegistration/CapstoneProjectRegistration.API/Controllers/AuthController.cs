using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.AccountRequest;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _service.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _service.LoginAsync(request);
            return Ok(token);
        }
    }
}
