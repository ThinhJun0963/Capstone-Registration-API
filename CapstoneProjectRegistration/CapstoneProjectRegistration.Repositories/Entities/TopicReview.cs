using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Repositories.Entities;

public class TopicReview
{
    [Key]
    public int Id { get; set; }

    public int TopicId { get; set; }

    public Topic Topic { get; set; } = null!;

    public int ReviewerId { get; set; }

    public Lecturer Reviewer { get; set; } = null!;

    [StringLength(20)]
    public string Decision { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public DateTime ReviewDate { get; set; }
}
