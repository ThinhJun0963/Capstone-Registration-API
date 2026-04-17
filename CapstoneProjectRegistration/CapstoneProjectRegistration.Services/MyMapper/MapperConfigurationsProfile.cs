using AutoMapper;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Services.Request.RegistrationPeriod;
using CapstoneProjectRegistration.Services.Request.Topic;
using CapstoneProjectRegistration.Services.Respond.RegistrationPeriod;
using CapstoneProjectRegistration.Services.Respond.Topics;

namespace CapstoneProjectRegistration.Services.MyMapper;

public class MapperConfigurationsProfile : Profile
{
    public MapperConfigurationsProfile()
    {
        CreateMap<TopicCreateRequest, Topic>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"))
            .ForMember(dest => dest.ReviewStatus, opt => opt.MapFrom(_ => "Pending"))
            .ForMember(dest => dest.PublicStatus, opt => opt.MapFrom(_ => "Private"));
        CreateMap<Topic, TopicRespond>();
        CreateMap<TopicUpdateRequest, Topic>();

        CreateMap<CreateRegistrationPeriodRequest, RegistrationPeriod>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"));
        CreateMap<RegistrationPeriod, RegistrationPeriodRespond>();
    }
}
