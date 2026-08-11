using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(IApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<ServiceResult<AuthenticatedUserDto>> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

        if (user is null)
            return ServiceResult<AuthenticatedUserDto>.Failure("Invalid username or password.");

        if (!user.IsActive)
            return ServiceResult<AuthenticatedUserDto>.Failure("This account has been deactivated.");

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
            return ServiceResult<AuthenticatedUserDto>.Failure("Invalid username or password.");

        var dto = new AuthenticatedUserDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            DoctorId = user.DoctorId,
            DepartmentId = user.DepartmentId,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return ServiceResult<AuthenticatedUserDto>.Success(dto);
    }

    public async Task RecordLoginAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return;

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
