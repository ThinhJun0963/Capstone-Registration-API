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

    public int SemesterId { get; set; }

    public Semester Semester { get; set; } = null!;

    public int CreatorId { get; set; }

    public Lecturer Creator { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    public ICollection<TopicReview> TopicReviews { get; set; } = new List<TopicReview>();
}
