using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Services.Request.RegistrationPeriod;

public class CreateRegistrationPeriodRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SemesterId { get; set; }
}
