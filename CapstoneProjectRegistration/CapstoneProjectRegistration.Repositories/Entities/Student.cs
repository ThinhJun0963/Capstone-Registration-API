using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneProjectRegistration.Repositories.Entities;

[Table("Student")]
public class Student
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string StudentCode { get; set; } = string.Empty;

    [StringLength(20)]
    public string GroupRole { get; set; } = "Member";

    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    public int? ApplicationUserId { get; set; }

    public ApplicationUser? ApplicationUser { get; set; }
}
