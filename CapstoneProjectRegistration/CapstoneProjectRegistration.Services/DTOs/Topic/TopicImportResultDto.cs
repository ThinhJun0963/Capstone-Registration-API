namespace CapstoneProjectRegistration.Services.DTOs.Topic;

public class TopicImportResultDto
{
    public string EnglishName { get; set; } = string.Empty;
    public string VietnameseName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> StudentCodes { get; set; } = new();
    public string SupervisorName { get; set; } = string.Empty;
    public string RawMarkdown { get; set; } = string.Empty;
}
