using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProjectRegistration.Repositories.Entities;

[Table("RegistrationPeriod")]
public class RegistrationPeriod
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Inactive";

    public int SemesterId { get; set; }

    public Semester Semester { get; set; } = null!;

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
