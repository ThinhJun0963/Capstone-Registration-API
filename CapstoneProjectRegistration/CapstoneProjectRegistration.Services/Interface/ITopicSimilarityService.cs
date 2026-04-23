using CapstoneProjectRegistration.Services.DTOs.Topic;

namespace CapstoneProjectRegistration.Services.Interface;

public interface ITopicSimilarityService
{
    Task<List<SimilarityResultDto>> CheckDuplicateAsync(DuplicateCheckRequestDto request);
}
