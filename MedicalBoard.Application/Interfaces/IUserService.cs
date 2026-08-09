using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IUserService
{
    Task<List<UserListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default);
}
