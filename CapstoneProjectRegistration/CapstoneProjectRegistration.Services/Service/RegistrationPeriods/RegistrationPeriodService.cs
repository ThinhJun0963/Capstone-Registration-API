using AutoMapper;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.RegistrationPeriod;
using CapstoneProjectRegistration.Services.Respond;
using CapstoneProjectRegistration.Services.Respond.RegistrationPeriod;

namespace CapstoneProjectRegistration.Services.Service.RegistrationPeriods;

public class RegistrationPeriodService : IRegistrationPeriodService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegistrationPeriodService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse> CreateAsync(CreateRegistrationPeriodRequest request)
    {
        if (request.StartDate >= request.EndDate)
        {
            return new ApiResponse().SetBadRequest(message: "StartDate must be before EndDate.");
        }

        var period = _mapper.Map<RegistrationPeriod>(request);
        await _unitOfWork.RegistrationPeriods.AddAsync(period);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse().SetOk("Registration period created.");
    }

    public async Task<ApiResponse> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        var period = await _unitOfWork.RegistrationPeriods.GetAsync(
            x => x.StartDate <= now && x.EndDate >= now && x.Status == "Active");

        if (period == null)
        {
            return new ApiResponse().SetNotFound(message: "No active registration period found.");
        }

        return new ApiResponse().SetOk(_mapper.Map<RegistrationPeriodRespond>(period));
    }
}
