using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProjectRegistration.Repositories.Entities;

[Table("Lecture")]
public class Lecturer
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(20)]
    public string Title { get; set; } = "Mr";

    [StringLength(255)]
    public string Specialization { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public ICollection<TopicReview> TopicReviews { get; set; } = new List<TopicReview>();
}
