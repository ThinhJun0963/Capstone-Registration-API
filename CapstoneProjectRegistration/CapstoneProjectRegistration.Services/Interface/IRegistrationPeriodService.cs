using CapstoneProjectRegistration.Services.Request.RegistrationPeriod;
using CapstoneProjectRegistration.Services.Respond;

namespace CapstoneProjectRegistration.Services.Interface;

public interface IRegistrationPeriodService
{
    Task<ApiResponse> CreateAsync(CreateRegistrationPeriodRequest request);
    Task<ApiResponse> GetActiveAsync();
}
