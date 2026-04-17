using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.RegistrationPeriod;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers;

[ApiController]
[Route("api/registration-periods")]
public class RegistrationPeriodsController : ControllerBase
{
    private readonly IRegistrationPeriodService _registrationPeriodService;

    public RegistrationPeriodsController(IRegistrationPeriodService registrationPeriodService)
    {
        _registrationPeriodService = registrationPeriodService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationPeriodRequest request)
    {
        var response = await _registrationPeriodService.CreateAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var response = await _registrationPeriodService.GetActiveAsync();
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }
}
