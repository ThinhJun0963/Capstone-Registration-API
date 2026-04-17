using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Services.Request.Topic;

public class TopicUpdateRequest
{
    [StringLength(255)]
    public string EnglishName { get; set; } = string.Empty;

    [StringLength(255)]
    public string VietnameseName { get; set; } = string.Empty;

    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    public int SemesterId { get; set; }
    public int RegistrationPeriodId { get; set; }
}
