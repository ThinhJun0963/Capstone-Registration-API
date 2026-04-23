namespace CapstoneProjectRegistration.Services.DTOs.Topic;

public class SimilarityResultDto
{
    public int TopicId { get; set; }
    public string EnglishName { get; set; } = string.Empty;
    public string SemesterName { get; set; } = string.Empty;
    public double TotalScore { get; set; }
    public double TfIdfScore { get; set; }
    public double FuzzyScore { get; set; }
    public string RiskLevel { get; set; } = "Low";
}
