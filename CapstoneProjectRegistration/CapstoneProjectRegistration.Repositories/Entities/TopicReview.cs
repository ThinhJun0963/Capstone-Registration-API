using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProjectRegistration.Repositories.Entities;

[Table("TopicReview")]
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

    public bool IsFinalized { get; set; }
}
