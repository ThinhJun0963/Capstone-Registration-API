using CapstoneProjectRegistration.Services.DTOs.Topic;

namespace CapstoneProjectRegistration.Services.Interface;

public interface ITopicImportService
{
    Task<TopicImportResultDto> ImportFromWordAsync(Stream fileStream);
}
