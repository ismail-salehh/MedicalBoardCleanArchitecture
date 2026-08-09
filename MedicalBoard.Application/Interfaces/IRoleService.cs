using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IRoleService
{
    Task<List<RoleListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> SaveAsync(SaveRoleDto dto, CancellationToken cancellationToken = default);
}
