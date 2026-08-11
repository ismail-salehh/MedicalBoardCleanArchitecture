using MedicalBoard.Application.Common;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Entities;
using MedicalBoard.Domain.Enums;
using MedicalBoard.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IApplicationDbContext _context;
    public AppointmentService(IApplicationDbContext context) => _context = context;

    public async Task<List<AppointmentDto>> GetAllAsync(
        int? doctorId = null,
        int? patientId = null,
        AppointmentStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .AsQueryable();

        if (doctorId.HasValue) query = query.Where(a => a.DoctorId == doctorId.Value);
        if (patientId.HasValue) query = query.Where(a => a.PatientId == patientId.Value);
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        if (fromDate.HasValue) query = query.Where(a => a.AppointmentDate >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(a => a.AppointmentDate <= toDate.Value);

        return await query
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                CancellationReason = a.CancellationReason
            })
            .ToListAsync();
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.Id == id)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                CancellationReason = a.CancellationReason
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<AppointmentDto>> GetTodaysAppointmentsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        return await GetAllAsync(fromDate: today, toDate: tomorrow);
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateAppointmentDto dto)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == dto.DoctorId);
        if (doctor is null)
            return ServiceResult<int>.Failure("Doctor not found.");
        if (!doctor.IsActive)
            return ServiceResult<int>.Failure("Cannot book an appointment with an inactive doctor.");

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == dto.PatientId);
        if (patient is null)
            return ServiceResult<int>.Failure("Patient not found.");
        if (!patient.IsActive)
            return ServiceResult<int>.Failure("Cannot book an appointment for an inactive patient.");

        // Proactive checks -- the unique indexes on (DoctorId, AppointmentDate) and
        // (PatientId, AppointmentDate) are the real guarantee, but we check here first
        // to return a friendly message instead of surfacing a raw DbUpdateException.
        var doctorBusy = await _context.Appointments.AnyAsync(a =>
            a.DoctorId == dto.DoctorId &&
            a.AppointmentDate == dto.AppointmentDate &&
            a.Status != AppointmentStatus.Cancelled);
        if (doctorBusy)
            return ServiceResult<int>.Failure("This doctor already has an appointment at that time.");

        var patientBusy = await _context.Appointments.AnyAsync(a =>
            a.PatientId == dto.PatientId &&
            a.AppointmentDate == dto.AppointmentDate &&
            a.Status != AppointmentStatus.Cancelled);
        if (patientBusy)
            return ServiceResult<int>.Failure("This patient already has an appointment at that time.");

        var appointment = new Appointment
        {
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            AppointmentDate = dto.AppointmentDate,
            Notes = dto.Notes,
            CreatedByUserId = dto.CreatedByUserId,
            Status = AppointmentStatus.Pending
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return ServiceResult<int>.Success(appointment.Id);
    }

    public async Task<ServiceResult> ConfirmAsync(int id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment is null)
            return ServiceResult.Failure("Appointment not found.");

        try
        {
            appointment.Confirm();
        }
        catch (DomainRuleViolationException ex)
        {
            return ServiceResult.Failure(ex.Message);
        }

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> CompleteAsync(int id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment is null)
            return ServiceResult.Failure("Appointment not found.");

        try
        {
            appointment.Complete();
        }
        catch (DomainRuleViolationException ex)
        {
            return ServiceResult.Failure(ex.Message);
        }

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> CancelAsync(CancelAppointmentDto dto)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == dto.Id);
        if (appointment is null)
            return ServiceResult.Failure("Appointment not found.");

        try
        {
            appointment.Cancel(dto.CancellationReason);
        }
        catch (DomainRuleViolationException ex)
        {
            return ServiceResult.Failure(ex.Message);
        }

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<Dictionary<AppointmentStatus, int>> GetCountsByStatusAsync()
    {
        var counts = await _context.Appointments
            .AsNoTracking()
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

        // Ensure every status has an entry (even 0) so the Index view's badge row never KeyNotFoundExceptions.
        foreach (AppointmentStatus status in Enum.GetValues<AppointmentStatus>())
            counts.TryAdd(status, 0);

        return counts;
    }
}