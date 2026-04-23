using CapstoneProjectRegistration.API.Security;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.RegistrationPeriod;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers;

[ApiController]
[Tags("03 · Registration periods")]
[Route("api/v1/registration-periods")]
public class RegistrationPeriodsController : ControllerBase
{
    private readonly IRegistrationPeriodService _registrationPeriodService;

    public RegistrationPeriodsController(IRegistrationPeriodService registrationPeriodService)
    {
        _registrationPeriodService = registrationPeriodService;
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.RegistrationPeriodsManage)]
    public async Task<IActionResult> Post([FromBody] CreateRegistrationPeriodRequest request)
    {
        var response = await _registrationPeriodService.CreateAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive()
    {
        var response = await _registrationPeriodService.GetActiveAsync();
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }
}
