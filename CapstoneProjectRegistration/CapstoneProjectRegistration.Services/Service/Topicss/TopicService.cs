using AutoMapper;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Topic;
using CapstoneProjectRegistration.Services.Respond;
using CapstoneProjectRegistration.Services.Respond.Topics;
using System.Security.Claims;

namespace CapstoneProjectRegistration.Services.Service.Topicss
{
    public class TopicService : ITopicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TopicService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse> AddNewTopic(TopicRequest topicRequest)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var topic = _mapper.Map<Topic>(topicRequest);
                var topicExist = await _unitOfWork.Lecturers.GetAsync(x => x.Id == topic.CreatorId);
                if (topicExist == null)
                {
                    await _unitOfWork.Topics.AddAsync(topic);
                    await _unitOfWork.SaveChangesAsync();

                }

                return apiResponse.SetBadRequest("Topic already exist!!!");
            }
            catch (Exception e)
            {
                return apiResponse.SetBadRequest(e.Message);
            }
        }

        public async Task<ApiResponse> DeleteTopicData(int Id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var topic = await _unitOfWork.Topics.GetAsync(c => c.Id == Id);
                if (topic == null)
                {
                    return apiResponse.SetNotFound("Can not found the Topics detail");
                }
                await _unitOfWork.Topics.RemoveByIdAsync(Id);
                await _unitOfWork.SaveChangesAsync();
                return apiResponse.SetOk("Deleled successfully!");


            }
            catch (Exception e)
            {
                return apiResponse.SetBadRequest(e.Message);
            }
        }

        public async Task<ApiResponse> GetAllTopic()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var topics = await _unitOfWork.Topics.GetAllAsync(null);
                var resTopics = _mapper.Map<List<TopicRespond>>(topics);
                return new ApiResponse().SetOk(resTopics);
            }
            catch (Exception e)
            {
                return apiResponse.SetBadRequest(e.Message);
            }
        }

        public async Task<ApiResponse> UpdateTopicData(int Id, TopicUpdateRequest topicUpdateRequest)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var topic = await _unitOfWork.Topics.GetAsync(c => c.Id == Id);
                if (topic == null)
                {
                    return apiResponse.SetNotFound("Can not found the Topics detail");
                }
                _mapper.Map(topicUpdateRequest, topic);
                await _unitOfWork.SaveChangesAsync();
                return apiResponse.SetOk("Topics's details updated successfully");

            }
            catch (Exception e)
            {
                return apiResponse.SetBadRequest(e.Message);
            }
        }
    }
}
