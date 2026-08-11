using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IPatientService
{
    Task<List<PatientListItemDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<PatientDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreatePatientDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateAsync(UpdatePatientDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default);

    // LINQ business task: search patients by name or phone.
    Task<List<PatientListItemDto>> SearchAsync(string term, CancellationToken cancellationToken = default);
}
