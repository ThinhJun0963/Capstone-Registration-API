namespace CapstoneProjectRegistration.Services.Respond.Topics;

public class TopicReviewSummaryRespond
{
    public int TopicId { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public string ReviewStatus { get; set; } = "Pending";
    public string PublicStatus { get; set; } = "Private";
}
