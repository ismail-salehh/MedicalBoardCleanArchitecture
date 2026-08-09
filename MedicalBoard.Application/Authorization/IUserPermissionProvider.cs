namespace MedicalBoard.Application.Authorization;

public interface IUserPermissionProvider
{
    Task<IReadOnlySet<string>> GetPermissionsAsync(int userId, CancellationToken cancellationToken = default);
}
