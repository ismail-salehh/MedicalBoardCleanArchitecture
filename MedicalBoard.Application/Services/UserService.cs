using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null) return null;

        return new UserDetailDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            DoctorId = user.DoctorId,
            DepartmentId = user.DepartmentId,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList()
        };
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var usernameTaken = await _context.Users.AnyAsync(u => u.Username == dto.Username, cancellationToken);
        if (usernameTaken)
            return ServiceResult<int>.Fail("Username is already taken.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            DoctorId = dto.DoctorId,
            DepartmentId = dto.DepartmentId,
            IsActive = true
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        foreach (var roleId in dto.RoleIds.Distinct())
            user.UserRoles.Add(new UserRole { RoleId = roleId });

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<int>.Ok(user.Id);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == dto.Id, cancellationToken);

        if (user is null)
            return ServiceResult.Fail("User not found.");

        user.Email = dto.Email;
        user.DoctorId = dto.DoctorId;
        user.DepartmentId = dto.DepartmentId;

        var newRoleIds = dto.RoleIds.Distinct().ToHashSet();
        var toRemove = user.UserRoles.Where(ur => !newRoleIds.Contains(ur.RoleId)).ToList();
        foreach (var ur in toRemove)
            user.UserRoles.Remove(ur);

        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
        foreach (var roleId in newRoleIds.Except(existingRoleIds))
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return ServiceResult.Fail("User not found.");

        user.IsActive = isActive;
        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
