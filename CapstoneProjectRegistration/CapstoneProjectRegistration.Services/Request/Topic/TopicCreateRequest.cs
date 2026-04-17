using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Services.Request.Topic;

public class TopicCreateRequest
{
    [Required]
    [StringLength(255)]
    public string EnglishName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string VietnameseName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string TopicCode { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    public int SemesterId { get; set; }
    public int RegistrationPeriodId { get; set; }
    public int CreatorId { get; set; }
}
