using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IApplicationDbContext _context;
    public DepartmentService(IApplicationDbContext context) => _context = context;

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                IsActive = d.IsActive,
                DoctorCount = d.Doctors.Count
            })
            .ToListAsync();
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                IsActive = d.IsActive,
                DoctorCount = d.Doctors.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateDepartmentDto dto)
    {
        var codeTaken = await _context.Departments.AnyAsync(d => d.Code == dto.Code);
        if (codeTaken)
            return ServiceResult<int>.Failure("A department with this code already exists.");

        var department = new Department
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            IsActive = true
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return ServiceResult<int>.Success(department.Id);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateDepartmentDto dto)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == dto.Id);
        if (department is null)
            return ServiceResult.Failure("Department not found.");

        var codeTaken = await _context.Departments.AnyAsync(d => d.Code == dto.Code && d.Id != dto.Id);
        if (codeTaken)
            return ServiceResult.Failure("A department with this code already exists.");

        department.Name = dto.Name;
        department.Code = dto.Code;
        department.Description = dto.Description;
        department.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (department is null)
            return ServiceResult.Failure("Department not found.");

        // Mirrors the doctor/patient pattern: history-preserving deactivation, not delete.
        department.IsActive = isActive;
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }
}