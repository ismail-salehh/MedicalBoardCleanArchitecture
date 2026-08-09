using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IPatientService
{
    Task<List<PatientDto>> SearchAsync(string? searchTerm = null);
    Task<PatientDto?> GetByIdAsync(int id);
    Task<ServiceResult<int>> CreateAsync(CreatePatientDto dto);
    Task<ServiceResult> UpdateAsync(UpdatePatientDto dto);
    Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive);
}
