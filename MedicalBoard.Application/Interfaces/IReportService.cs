using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IReportService
{
    Task<List<AppointmentsByDoctorDto>> GetAppointmentsByDoctorAsync(CancellationToken cancellationToken = default);
    Task<List<AppointmentsByDepartmentDto>> GetAppointmentsByDepartmentAsync(CancellationToken cancellationToken = default);
    Task<List<AppointmentsByStatusDto>> GetAppointmentsByStatusAsync(CancellationToken cancellationToken = default);
    Task<List<DoctorNoAppointmentsTodayDto>> GetDoctorsWithNoAppointmentsTodayAsync(CancellationToken cancellationToken = default);
    Task<List<BusiestDoctorDto>> GetBusiestDoctorsAsync(int top = 5, CancellationToken cancellationToken = default);
}
