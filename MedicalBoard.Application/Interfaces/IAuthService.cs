using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;

namespace MedicalBoard.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthenticatedUserDto>> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
    Task RecordLoginAsync(int userId, CancellationToken cancellationToken = default);
}