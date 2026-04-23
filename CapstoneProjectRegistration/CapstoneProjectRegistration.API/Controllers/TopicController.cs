using CapstoneProjectRegistration.Services.DTOs.Topic;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Topic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers
{
    [ApiController]
    [Route("api/topics")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly ITopicImportService _topicImportService;
        private readonly ITopicSimilarityService _topicSimilarityService;

        public TopicController(
            ITopicService topicService,
            ITopicImportService topicImportService,
            ITopicSimilarityService topicSimilarityService)
        {
            _topicService = topicService;
            _topicImportService = topicImportService;
            _topicSimilarityService = topicSimilarityService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTopic([FromBody] TopicCreateRequest request)
        {
            var result = await _topicService.CreateTopicAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTopic()
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
        [Authorize]
        public async Task<IActionResult> DeleteTopicDetail(int id)
        {
            var response = await _topicService.DeleteTopicAsync(id);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateTopicData(int id, [FromBody] TopicUpdateRequest topicUpdateRequest)
        {
            var response = await _topicService.UpdateTopicAsync(id, topicUpdateRequest);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{id:int}/assign-reviewers")]
        [Authorize]
        public async Task<IActionResult> AssignReviewers(int id, [FromBody] AssignReviewersRequest request)
        {
            var result = await _topicService.AssignReviewersAsync(id, request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:int}/reviews")]
        [Authorize]
        public async Task<IActionResult> SubmitReview(int id, [FromBody] SubmitTopicReviewRequest request)
        {
            var result = await _topicService.SubmitReviewAsync(id, request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}/review-summary")]
        [Authorize]
        public async Task<IActionResult> ReviewSummary(int id)
        {
            var result = await _topicService.GetReviewSummaryAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost("{id:int}/publish")]
        [Authorize]
        public async Task<IActionResult> Publish(int id)
        {
            var result = await _topicService.PublishTopicAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("extract-from-file")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> ExtractFromFile([FromForm] ExtractTopicFromFileRequest request)
        {
            var result = await _topicService.ExtractTopicFromFileAsync(request.File);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Import topic fields from a .docx file using Word-to-Markdown conversion and pattern extraction.
        /// </summary>
        [HttpPost("import-word")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> ImportWord([FromForm] ImportWordRequest request)
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
        [HttpPost("check-similarity")]
        [Authorize]
        public async Task<IActionResult> CheckSimilarity([FromBody] DuplicateCheckRequestDto request)
        {
            var result = await _topicSimilarityService.CheckDuplicateAsync(request);
            return Ok(result);
        }
    }
}
