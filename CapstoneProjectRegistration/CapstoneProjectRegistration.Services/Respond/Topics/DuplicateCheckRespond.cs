namespace CapstoneProjectRegistration.Services.Respond.Topics;

public class DuplicateCheckRespond
{
    public bool IsDuplicateSuspected { get; set; }
    public string RiskLevel { get; set; } = "Low";
    public double SimilarityScore { get; set; }
    public string Message { get; set; } = string.Empty;
}
