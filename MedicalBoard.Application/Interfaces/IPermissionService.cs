using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IPermissionService
{
    Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
