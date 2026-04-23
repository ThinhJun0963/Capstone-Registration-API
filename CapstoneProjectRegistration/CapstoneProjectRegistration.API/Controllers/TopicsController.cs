using CapstoneProjectRegistration.API.Security;
using CapstoneProjectRegistration.Services.DTOs.Topic;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Topic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers;

[ApiController]
[Tags("02 · Topics — catalog & workflow")]
[Route("api/v1/topics")]
public class TopicsController : ControllerBase
{
    private readonly ITopicService _topicService;
    private readonly ITopicImportService _topicImportService;
    private readonly ITopicSimilarityService _topicSimilarityService;

    public TopicsController(
        ITopicService topicService,
        ITopicImportService topicImportService,
        ITopicSimilarityService topicSimilarityService)
    {
        _topicService = topicService;
        _topicImportService = topicImportService;
        _topicSimilarityService = topicSimilarityService;
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.TopicsCreate)]
    public async Task<IActionResult> Post([FromBody] TopicCreateRequest request)
    {
        var result = await _topicService.CreateTopicAsync(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }


    [Authorize(Roles = "Lecturer")]
    [HttpGet]
    //[AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var response = await _topicService.GetAllTopicsAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _topicService.GetTopicByIdAsync(id);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.TopicsDelete)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _topicService.DeleteTopicAsync(id);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.TopicsUpdate)]
    public async Task<IActionResult> Put(int id, [FromBody] TopicUpdateRequest topicUpdateRequest)
    {
        var response = await _topicService.UpdateTopicAsync(id, topicUpdateRequest);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id:int}/reviewers")]
    [Authorize(Policy = AuthPolicies.TopicsAssignReviewers)]
    public async Task<IActionResult> PutReviewers(int id, [FromBody] AssignReviewersRequest request)
    {
        var result = await _topicService.AssignReviewersAsync(id, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/reviews")]
    [Authorize(Policy = AuthPolicies.TopicsSubmitReview)]
    public async Task<IActionResult> PostReview(int id, [FromBody] SubmitTopicReviewRequest request)
    {
        var result = await _topicService.SubmitReviewAsync(id, request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}/reviews/summary")]
    [Authorize(Policy = AuthPolicies.TopicsReadReviewSummary)]
    public async Task<IActionResult> GetReviewSummary(int id)
    {
        var result = await _topicService.GetReviewSummaryAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id:int}/publication")]
    [Authorize(Policy = AuthPolicies.TopicsPublish)]
    public async Task<IActionResult> PostPublication(int id)
    {
        var result = await _topicService.PublishTopicAsync(id);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("import/preview")]
    [Consumes("multipart/form-data")]
    [Authorize(Policy = AuthPolicies.TopicsImport)]
    public async Task<IActionResult> PostImportPreview([FromForm] ExtractTopicFromFileRequest request)
    {
        var result = await _topicService.ExtractTopicFromFileAsync(request.File);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Import topic fields from a .docx file using Word-to-Markdown conversion and pattern extraction.
    /// </summary>
    [HttpPost("import/word")]
    [Consumes("multipart/form-data")]
    [Authorize(Policy = AuthPolicies.TopicsImport)]
    public async Task<IActionResult> PostImportWord([FromForm] ImportWordRequest request)
    {
        var file = request?.File;
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".docx")
        {
            return BadRequest("Only .docx files are supported.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _topicImportService.ImportFromWordAsync(stream);
        return Ok(result);
    }

    /// <summary>
    /// Advanced duplicate / similarity check (TF-IDF + fuzzy name match). Returns the top 5 similar topics.
    /// </summary>
    [HttpPost("similarity-checks")]
    [Authorize(Policy = AuthPolicies.TopicsSimilarityCheck)]
    public async Task<IActionResult> PostSimilarityCheck([FromBody] DuplicateCheckRequestDto request)
    {
        var result = await _topicSimilarityService.CheckDuplicateAsync(request);
        return Ok(result);
    }
}
