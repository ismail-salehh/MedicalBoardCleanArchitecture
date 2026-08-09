using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Enums;

public class AppointmentService : IAppointmentService
{
    public Task<ServiceResult> CancelAsync(CancelAppointmentDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> CompleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> ConfirmAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<int>> CreateAsync(CreateAppointmentDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<List<AppointmentDto>> GetAllAsync(int? doctorId = null, int? patientId = null, AppointmentStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        throw new NotImplementedException();
    }

    public Task<AppointmentDto?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<AppointmentStatus, int>> GetCountsByStatusAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<AppointmentDto>> GetTodaysAppointmentsAsync()
    {
        throw new NotImplementedException();
    }
}