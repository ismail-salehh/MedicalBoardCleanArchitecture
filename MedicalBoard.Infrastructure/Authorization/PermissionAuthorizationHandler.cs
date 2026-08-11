using System.Security.Claims;
using MedicalBoard.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace MedicalBoard.Infrastructure.Authorization;

// Stage 4 -- Permission authorization. Reads the DB-backed permission set for the
// signed-in user and succeeds the requirement when the required code is present.
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserPermissionProvider _permissionProvider;

    public PermissionAuthorizationHandler(IUserPermissionProvider permissionProvider)
    {
        _permissionProvider = permissionProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
            return;

        var permissions = await _permissionProvider.GetPermissionsAsync(userId);
        if (permissions.Contains(requirement.PermissionCode))
            context.Succeed(requirement);
    }
}