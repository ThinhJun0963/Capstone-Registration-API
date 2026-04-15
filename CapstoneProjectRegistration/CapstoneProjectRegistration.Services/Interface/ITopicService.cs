using CapstoneProjectRegistration.Services.Request.Topic;
using CapstoneProjectRegistration.Services.Respond;
using MaxMind.GeoIP2.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProjectRegistration.Services.Interface
{
    public interface ITopicService
    {
        Task<ApiResponse> AddNewTopic(TopicRequest topicRequest);
        Task<ApiResponse> GetAllTopic();
        Task<ApiResponse> DeleteTopicData(int Id);
        Task<ApiResponse> UpdateTopicData(int Id, TopicUpdateRequest topicUpdateRequest);
    }
}
