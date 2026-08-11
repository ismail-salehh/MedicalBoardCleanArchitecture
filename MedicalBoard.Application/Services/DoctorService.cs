using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IApplicationDbContext _context;
    public DoctorService(IApplicationDbContext context) => _context = context;

    public async Task<List<DoctorListItemDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Doctors.AsNoTracking().Include(d => d.Department).AsQueryable();
        if (!includeInactive)
            query = query.Where(d => d.IsActive);

        return await query
            .OrderBy(d => d.FullName)
            .Select(d => new DoctorListItemDto
            {
                Id = d.Id,
                EmployeeNumber = d.EmployeeNumber,
                FullName = d.FullName,
                Specialty = d.Specialty,
                DepartmentName = d.Department.Name,
                IsActive = d.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DoctorListItemDto>> GetActiveDoctorsAsync(CancellationToken cancellationToken = default)
        => await GetAllAsync(includeInactive: false, cancellationToken);

    public async Task<DoctorDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doctor is null) return null;

        return new DoctorDetailDto
        {
            Id = doctor.Id,
            EmployeeNumber = doctor.EmployeeNumber,
            FullName = doctor.FullName,
            Specialty = doctor.Specialty,
            DepartmentName = doctor.Department.Name,
            DepartmentId = doctor.DepartmentId,
            Phone = doctor.Phone,
            Email = doctor.Email,
            IsActive = doctor.IsActive,
            CreatedAt = doctor.CreatedAt
        };
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateDoctorDto dto, CancellationToken cancellationToken = default)
    {
        var employeeNumberTaken = await _context.Doctors.AnyAsync(d => d.EmployeeNumber == dto.EmployeeNumber, cancellationToken);
        if (employeeNumberTaken)
            return ServiceResult<int>.Failure("This employee number is already in use.");

        // Business rule: a doctor must belong to an active department.
        var department = await _context.Departments.FirstOrDefaultAsync(dep => dep.Id == dto.DepartmentId, cancellationToken);
        if (department is null)
            return ServiceResult<int>.Failure("Department not found.");
        if (!department.IsActive)
            return ServiceResult<int>.Failure("Cannot assign a doctor to an inactive department.");

        var doctor = new Doctor
        {
            EmployeeNumber = dto.EmployeeNumber,
            FullName = dto.FullName,
            Specialty = dto.Specialty,
            Phone = dto.Phone,
            Email = dto.Email,
            DepartmentId = dto.DepartmentId,
            IsActive = true
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<int>.Success(doctor.Id);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateDoctorDto dto, CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == dto.Id, cancellationToken);
        if (doctor is null)
            return ServiceResult.Failure("Doctor not found.");

        var department = await _context.Departments.FirstOrDefaultAsync(dep => dep.Id == dto.DepartmentId, cancellationToken);
        if (department is null)
            return ServiceResult.Failure("Department not found.");
        if (!department.IsActive)
            return ServiceResult.Failure("Cannot assign a doctor to an inactive department.");

        doctor.FullName = dto.FullName;
        doctor.Specialty = dto.Specialty;
        doctor.Phone = dto.Phone;
        doctor.Email = dto.Email;
        doctor.DepartmentId = dto.DepartmentId;

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (doctor is null)
            return ServiceResult.Failure("Doctor not found.");

        // History-preserving: deactivate rather than delete. Existing appointments remain intact.
        doctor.IsActive = isActive;
        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success();
    }
}
