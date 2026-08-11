using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class PatientService : IPatientService
{
    private readonly IApplicationDbContext _context;
    public PatientService(IApplicationDbContext context) => _context = context;

    public async Task<List<PatientListItemDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Patients.AsNoTracking().AsQueryable();
        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query
            .OrderBy(p => p.FullName)
            .Select(p => new PatientListItemDto
            {
                Id = p.Id,
                PatientNumber = p.PatientNumber,
                FullName = p.FullName,
                Phone = p.Phone,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PatientDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (patient is null) return null;

        return new PatientDetailDto
        {
            Id = patient.Id,
            PatientNumber = patient.PatientNumber,
            FullName = patient.FullName,
            NationalIdentifier = patient.NationalIdentifier,
            DateOfBirth = patient.DateOfBirth,
            Phone = patient.Phone,
            Email = patient.Email,
            IsActive = patient.IsActive,
            CreatedAt = patient.CreatedAt
        };
    }

    public async Task<ServiceResult<int>> CreateAsync(CreatePatientDto dto, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(dto.NationalIdentifier))
        {
            var idTaken = await _context.Patients.AnyAsync(p => p.NationalIdentifier == dto.NationalIdentifier, cancellationToken);
            if (idTaken)
                return ServiceResult<int>.Failure("A patient with this national/external identifier already exists.");
        }

        var patient = new Patient
        {
            FullName = dto.FullName,
            NationalIdentifier = dto.NationalIdentifier,
            DateOfBirth = dto.DateOfBirth,
            Phone = dto.Phone,
            Email = dto.Email,
            IsActive = true,
            // Placeholder -- replaced with a stable, sequential number below once the Id is known.
            PatientNumber = string.Empty
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);

        patient.PatientNumber = $"P{patient.Id:D6}";
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<int>.Success(patient.Id);
    }

    public async Task<ServiceResult> UpdateAsync(UpdatePatientDto dto, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == dto.Id, cancellationToken);
        if (patient is null)
            return ServiceResult.Failure("Patient not found.");

        if (!string.IsNullOrWhiteSpace(dto.NationalIdentifier))
        {
            var idTaken = await _context.Patients.AnyAsync(
                p => p.NationalIdentifier == dto.NationalIdentifier && p.Id != dto.Id, cancellationToken);
            if (idTaken)
                return ServiceResult.Failure("A patient with this national/external identifier already exists.");
        }

        patient.FullName = dto.FullName;
        patient.NationalIdentifier = dto.NationalIdentifier;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Phone = dto.Phone;
        patient.Email = dto.Email;

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SetActiveStatusAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (patient is null)
            return ServiceResult.Failure("Patient not found.");

        // History-preserving: deactivate rather than delete, so past appointments stay intact.
        patient.IsActive = isActive;
        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success();
    }

    // LINQ business task: search patients by name or phone.
    public async Task<List<PatientListItemDto>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
            return await GetAllAsync(cancellationToken: cancellationToken);

        var pattern = term.Trim();

        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.IsActive && (
                p.FullName.Contains(pattern) ||
                (p.Phone != null && p.Phone.Contains(pattern))))
            .OrderBy(p => p.FullName)
            .Select(p => new PatientListItemDto
            {
                Id = p.Id,
                PatientNumber = p.PatientNumber,
                FullName = p.FullName,
                Phone = p.Phone,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
