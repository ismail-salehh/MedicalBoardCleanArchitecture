using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IDoctorService
{
    Task<List<DoctorDto>> GetAllAsync(int? departmentId = null, bool? isActive = null);
    Task<DoctorDto?> GetByIdAsync(int id);
    Task<ServiceResult<int>> CreateAsync(CreateDoctorDto dto);
    Task<ServiceResult> UpdateAsync(UpdateDoctorDto dto);
    Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive);
    Task<List<DoctorDto>> GetDoctorsWithNoAppointmentsTodayAsync();
    Task<List<(DoctorDto Doctor, int AppointmentCount)>> GetBusiestDoctorsAsync(int top = 5);
}
