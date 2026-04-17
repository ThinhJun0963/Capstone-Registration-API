namespace CapstoneProjectRegistration.Services.Respond.RegistrationPeriod;

public class RegistrationPeriodRespond
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SemesterId { get; set; }
}
