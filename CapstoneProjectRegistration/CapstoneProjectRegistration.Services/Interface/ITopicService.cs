using CapstoneProjectRegistration.Services.Request.Topic;
using CapstoneProjectRegistration.Services.Respond;
using Microsoft.AspNetCore.Http;

namespace CapstoneProjectRegistration.Services.Interface;

public interface ITopicService
{
    Task<ApiResponse> CreateTopicAsync(TopicCreateRequest request);
    Task<ApiResponse> GetAllTopicsAsync();
    Task<ApiResponse> GetTopicByIdAsync(int id);
    Task<ApiResponse> UpdateTopicAsync(int id, TopicUpdateRequest request);
    Task<ApiResponse> DeleteTopicAsync(int id);
    Task<ApiResponse> AssignReviewersAsync(int topicId, AssignReviewersRequest request);
    Task<ApiResponse> SubmitReviewAsync(int topicId, SubmitTopicReviewRequest request);
    Task<ApiResponse> GetReviewSummaryAsync(int topicId);
    Task<ApiResponse> PublishTopicAsync(int topicId);
    Task<ApiResponse> CheckDuplicateAsync(TopicDuplicateCheckRequest request);
    Task<ApiResponse> ExtractTopicFromFileAsync(IFormFile file);
}
