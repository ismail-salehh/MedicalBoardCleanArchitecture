using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;
    public PermissionService(IApplicationDbContext context) => _context = context;

    public async Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AsNoTracking()
            .OrderBy(p => p.Code)
            .Select(p => new PermissionDto { Id = p.Id, Code = p.Code, Name = p.Name, Description = p.Description })
            .ToListAsync(cancellationToken);
    }
}
