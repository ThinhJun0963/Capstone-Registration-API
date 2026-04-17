using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Topic;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers
{
    [ApiController]
    [Route("api/topics")]
    public class TopicController : ControllerBase
    {
        public ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] TopicCreateRequest request)
        {
            var result = await _topicService.CreateTopicAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopic()
        {
            var response = await _topicService.GetAllTopicsAsync();
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _topicService.GetTopicByIdAsync(id);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTopicDetail(int id)
        {
            var response = await _topicService.DeleteTopicAsync(id);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTopicData(int id, TopicUpdateRequest topicUpdateRequest)
        {
            var resposne = await _topicService.UpdateTopicAsync(id, topicUpdateRequest);
            return resposne.IsSuccess ? Ok(resposne) : BadRequest(resposne);
        }

        [HttpPost("{id:int}/assign-reviewers")]
        public async Task<IActionResult> AssignReviewers(int id, [FromBody] AssignReviewersRequest request)
        {
            var response = await _topicService.AssignReviewersAsync(id, request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{id:int}/reviews")]
        public async Task<IActionResult> SubmitReview(int id, [FromBody] SubmitTopicReviewRequest request)
        {
            var response = await _topicService.SubmitReviewAsync(id, request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id:int}/review-summary")]
        public async Task<IActionResult> ReviewSummary(int id)
        {
            var response = await _topicService.GetReviewSummaryAsync(id);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            var response = await _topicService.PublishTopicAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("duplicate-check")]
        public async Task<IActionResult> DuplicateCheck([FromBody] TopicDuplicateCheckRequest request)
        {
            var response = await _topicService.CheckDuplicateAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("extract-from-file")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ExtractFromFile([FromForm] ExtractTopicFromFileRequest request)
        {
            var response = await _topicService.ExtractTopicFromFileAsync(request.File);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
