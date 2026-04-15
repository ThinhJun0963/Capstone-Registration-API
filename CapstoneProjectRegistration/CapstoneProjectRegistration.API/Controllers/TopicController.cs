using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Topic;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicController : ControllerBase
    {
        public ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }


        [Route("AddNewTopic")]
        [HttpPost]

        public async Task<IActionResult> AddNewTopic(TopicRequest topicRequest)
        {

            var result = await _topicService.AddNewTopic(topicRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        [HttpGet("GetAllTopic")]
        public async Task<IActionResult> GetAllTopic()
        {
            var response = await _topicService.GetAllTopic();
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("DeleteTopicDetail/{id}")]
        public async Task<IActionResult> DeleteTopicDetail(int id)
        {
            var response = await _topicService.DeleteTopicData(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }


        [HttpPut("UpdateTopicData/{id}")]
        public async Task<IActionResult> UpdateTopicData(int id, TopicUpdateRequest topicUpdateRequest)
        {
            var resposne = await _topicService.UpdateTopicData(id, topicUpdateRequest);
            return resposne.IsSuccess ? Ok(resposne) : BadRequest(resposne);
        }
    }
}
