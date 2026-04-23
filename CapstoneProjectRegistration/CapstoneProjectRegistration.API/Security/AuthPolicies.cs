namespace CapstoneProjectRegistration.API.Security;

/// <summary>Authorization policy names (feature-based RBAC).</summary>
public static class AuthPolicies
{
    public const string RegistrationPeriodsManage = "RegistrationPeriods.Manage";

    public const string TopicsCreate = "Topics.Create";

    public const string TopicsUpdate = "Topics.Update";

    public const string TopicsDelete = "Topics.Delete";

    public const string TopicsAssignReviewers = "Topics.AssignReviewers";

    public const string TopicsSubmitReview = "Topics.SubmitReview";

    public const string TopicsReadReviewSummary = "Topics.ReadReviewSummary";

    public const string TopicsPublish = "Topics.Publish";

    public const string TopicsImport = "Topics.Import";

    public const string TopicsSimilarityCheck = "Topics.SimilarityCheck";
}
