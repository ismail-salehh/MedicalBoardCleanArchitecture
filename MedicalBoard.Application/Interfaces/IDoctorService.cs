using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IDoctorService
{
    Task<List<DoctorListItemDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<DoctorDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateDoctorDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateAsync(UpdateDoctorDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default);

    // LINQ business task: doctors with no appointments today lives in IReportService;
    // this one supports the appointment-create screen (only active doctors are selectable).
    Task<List<DoctorListItemDto>> GetActiveDoctorsAsync(CancellationToken cancellationToken = default);
}
