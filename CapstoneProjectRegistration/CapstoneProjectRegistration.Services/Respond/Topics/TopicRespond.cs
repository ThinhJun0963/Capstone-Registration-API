using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Services.Respond.Topics;

public class TopicRespond
{
    public int Id { get; set; }

    [StringLength(255)]
    public string EnglishName { get; set; } = string.Empty;

    [StringLength(255)]
    public string VietnameseName { get; set; } = string.Empty;

    [StringLength(20)]
    public string TopicCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int SemesterId { get; set; }

    public int RegistrationPeriodId { get; set; }

    public int CreatorId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    [StringLength(20)]
    public string ReviewStatus { get; set; } = string.Empty;

    [StringLength(20)]
    public string PublicStatus { get; set; } = string.Empty;
}
