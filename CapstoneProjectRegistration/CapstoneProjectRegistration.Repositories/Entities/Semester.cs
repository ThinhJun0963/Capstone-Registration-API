using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Repositories.Entities;

public class Semester
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
