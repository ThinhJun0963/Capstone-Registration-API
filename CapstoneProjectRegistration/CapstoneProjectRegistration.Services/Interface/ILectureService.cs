using CapstoneProjectRegistration.Services.Request.Lecturer;
using CapstoneProjectRegistration.Services.Respond;

namespace CapstoneProjectRegistration.Services.Interface;

public interface ILectureService
{
    Task<ApiResponse> CreateAsync(LecturerCreateRequest request);
    Task<ApiResponse> GetAllAsync();
    Task<ApiResponse> GetByIdAsync(int id);
    Task<ApiResponse> UpdateAsync(int id, LecturerCreateRequest request);
    Task<ApiResponse> DeleteAsync(int id);
}
