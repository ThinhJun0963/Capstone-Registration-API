using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Repositories.Entities;

public class Student
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
    public string Status { get; set; } = string.Empty;
}
