namespace CapstoneProjectRegistration.Services.DTOs.Topic;

public class DuplicateCheckRequestDto
{
    public string EnglishName { get; set; } = string.Empty;
    public string VietnameseName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CurrentSemesterId { get; set; }
}
