using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class RoleService : IRoleService
{
    private readonly IApplicationDbContext _context;
    public RoleService(IApplicationDbContext context) => _context = context;

    public async Task<List<RoleListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AsNoTracking()
            .Select(r => new RoleListItemDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                UserCount = r.UserRoles.Count
            })
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .AsNoTracking()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null) return null;

        return new RoleDetailDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            PermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList()
        };
    }

    public async Task<ServiceResult<int>> SaveAsync(SaveRoleDto dto, CancellationToken cancellationToken = default)
    {
        var nameTaken = await _context.Roles.AnyAsync(r => r.Name == dto.Name && r.Id != dto.Id, cancellationToken);
        if (nameTaken)
            return ServiceResult<int>.Failure("A role with this name already exists.");

        Role role;
        if (dto.Id.HasValue)
        {
            var existing = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == dto.Id.Value, cancellationToken);

            if (existing is null)
                return ServiceResult<int>.Failure("Role not found.");

            role = existing;
            role.Name = dto.Name;
            role.Description = dto.Description;

            var newPermissionIds = dto.PermissionIds.Distinct().ToHashSet();
            var toRemove = role.RolePermissions.Where(rp => !newPermissionIds.Contains(rp.PermissionId)).ToList();
            foreach (var rp in toRemove)
                role.RolePermissions.Remove(rp);

            var existingIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
            foreach (var permissionId in newPermissionIds.Except(existingIds))
                role.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
        }
        else
        {
            role = new Role { Name = dto.Name, Description = dto.Description, IsActive = true };
            foreach (var permissionId in dto.PermissionIds.Distinct())
                role.RolePermissions.Add(new RolePermission { PermissionId = permissionId });

            _context.Roles.Add(role);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<int>.Success(role.Id);
    }
}
