using MedicalBoard.Application.DTOs;
using MedicalBoard.Domain.Enums;
using MedicalBoard.Application.Common;

namespace MedicalBoard.Application.Interfaces;

public interface IAppointmentService
{
    Task<List<AppointmentDto>> GetAllAsync(
        int? doctorId = null,
        int? patientId = null,
        AppointmentStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<List<AppointmentDto>> GetTodaysAppointmentsAsync();

    Task<ServiceResult<int>> CreateAsync(CreateAppointmentDto dto);
    Task<ServiceResult> ConfirmAsync(int id);
    Task<ServiceResult> CompleteAsync(int id);
    Task<ServiceResult> CancelAsync(CancelAppointmentDto dto);

    Task<Dictionary<AppointmentStatus, int>> GetCountsByStatusAsync();
}
