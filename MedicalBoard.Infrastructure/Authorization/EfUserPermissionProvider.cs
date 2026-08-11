using MedicalBoard.Application.Authorization;
using MedicalBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Infrastructure.Authorization;

public class EfUserPermissionProvider : IUserPermissionProvider
{
    private readonly ApplicationDbContext _context;
    public EfUserPermissionProvider(ApplicationDbContext context) => _context = context;

    public async Task<IReadOnlySet<string>> GetPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var codes = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Where(rp => rp.Permission.IsActive)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

        return codes.ToHashSet();
    }
}