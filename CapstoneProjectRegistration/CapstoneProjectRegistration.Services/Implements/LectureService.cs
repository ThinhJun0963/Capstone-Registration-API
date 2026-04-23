using AutoMapper;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Lecturer;
using CapstoneProjectRegistration.Services.Respond;

namespace CapstoneProjectRegistration.Services.Implements;

public class LectureService : ILectureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LectureService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse> CreateAsync(LecturerCreateRequest request)
    {
        var existing = await _unitOfWork.Lecturers.GetAsync(l => l.Email == request.Email);
        if (existing != null)
        {
            return new ApiResponse().SetBadRequest(message: "Lecturer with same email already exists.");
        }

        var lecturer = _mapper.Map<Lecturer>(request);
        await _unitOfWork.Lecturers.AddAsync(lecturer);
        await _unitOfWork.SaveChangesAsync();
        return new ApiResponse().SetOk("Lecturer created.");
    }

    public async Task<ApiResponse> GetAllAsync()
    {
        var list = await _unitOfWork.Lecturers.GetAllAsync();
        return new ApiResponse().SetOk(list);
    }

    public async Task<ApiResponse> GetByIdAsync(int id)
    {
        var lect = await _unitOfWork.Lecturers.GetByIdAsync(id);
        if (lect == null) return new ApiResponse().SetNotFound(message: "Lecturer not found.");
        return new ApiResponse().SetOk(lect);
    }

    public async Task<ApiResponse> UpdateAsync(int id, LecturerCreateRequest request)
    {
        var lect = await _unitOfWork.Lecturers.GetByIdAsync(id);
        if (lect == null) return new ApiResponse().SetNotFound(message: "Lecturer not found.");

        _mapper.Map(request, lect);
        _unitOfWork.Lecturers.Update(lect);
        await _unitOfWork.SaveChangesAsync();
        return new ApiResponse().SetOk("Lecturer updated.");
    }

    public async Task<ApiResponse> DeleteAsync(int id)
    {
        var lect = await _unitOfWork.Lecturers.GetByIdAsync(id);
        if (lect == null) return new ApiResponse().SetNotFound(message: "Lecturer not found.");

        _unitOfWork.Lecturers.Delete(lect);
        await _unitOfWork.SaveChangesAsync();
        return new ApiResponse().SetOk("Lecturer deleted.");
    }
}
