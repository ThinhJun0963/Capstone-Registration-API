using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Services.Request.Topic;

public class TopicDuplicateCheckRequest
{
    [Required]
    public string EnglishName { get; set; } = string.Empty;

    [Required]
    public string VietnameseName { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}
