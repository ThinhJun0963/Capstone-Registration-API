using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProjectRegistration.Repositories.Entities;

[Table("Topic")]
public class Topic
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string EnglishName { get; set; } = string.Empty;

    [StringLength(255)]
    public string VietnameseName { get; set; } = string.Empty;

    [StringLength(20)]
    public string TopicCode { get; set; } = string.Empty;

    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    public int SemesterId { get; set; }

    public Semester Semester { get; set; } = null!;

    public int RegistrationPeriodId { get; set; }

    public RegistrationPeriod RegistrationPeriod { get; set; } = null!;

    public int CreatorId { get; set; }

    public Lecturer Creator { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    [StringLength(20)]
    public string ReviewStatus { get; set; } = "Pending";

    [StringLength(20)]
    public string PublicStatus { get; set; } = "Private";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TopicReview> TopicReviews { get; set; } = new List<TopicReview>();
}
