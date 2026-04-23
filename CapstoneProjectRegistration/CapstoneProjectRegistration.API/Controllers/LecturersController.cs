using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Lecturer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers;

[ApiController]
[Route("api/lecturers")]
public class LecturersController : ControllerBase
{
    private readonly ILectureService _lectureService;

    public LecturersController(ILectureService lectureService)
    {
        _lectureService = lectureService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] LecturerCreateRequest request)
    {
        var response = await _lectureService.CreateAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var response = await _lectureService.GetAllAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _lectureService.GetByIdAsync(id);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] LecturerCreateRequest request)
    {
        var response = await _lectureService.UpdateAsync(id, request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _lectureService.DeleteAsync(id);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }
}
