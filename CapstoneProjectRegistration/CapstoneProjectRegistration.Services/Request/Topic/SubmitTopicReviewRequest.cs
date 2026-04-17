using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Services.Request.Topic;

public class SubmitTopicReviewRequest
{
    public int ReviewerId { get; set; }

    [Required]
    [StringLength(20)]
    public string Decision { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;
}
