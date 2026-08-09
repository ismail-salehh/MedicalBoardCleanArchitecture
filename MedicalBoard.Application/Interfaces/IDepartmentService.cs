using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<ServiceResult<int>> CreateAsync(CreateDepartmentDto dto);
    Task<ServiceResult> UpdateAsync(UpdateDepartmentDto dto);
    Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive);
}
