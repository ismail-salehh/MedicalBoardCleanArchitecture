using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;

public class DepartmentService : IDepartmentService
{
    public Task<ServiceResult<int>> CreateAsync(CreateDepartmentDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<List<DepartmentDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<DepartmentDto?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> UpdateAsync(UpdateDepartmentDto dto)
    {
        throw new NotImplementedException();
    }
}