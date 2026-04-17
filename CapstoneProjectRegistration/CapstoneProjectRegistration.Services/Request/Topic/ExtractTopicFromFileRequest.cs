using Microsoft.AspNetCore.Http;

namespace CapstoneProjectRegistration.Services.Request.Topic;

public class ExtractTopicFromFileRequest
{
    public IFormFile File { get; set; } = null!;
}
