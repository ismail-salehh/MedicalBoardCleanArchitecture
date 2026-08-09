using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Application.Services;

public class ReportService : IReportService
{
    private readonly IApplicationDbContext _context;
    public ReportService(IApplicationDbContext context) => _context = context;

    public async Task<List<AppointmentsByDoctorDto>> GetAppointmentsByDoctorAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .GroupBy(a => new { a.DoctorId, a.Doctor.FullName })
            .Select(g => new AppointmentsByDoctorDto
            {
                DoctorId = g.Key.DoctorId,
                DoctorName = g.Key.FullName,
                AppointmentCount = g.Count()
            })
            .OrderByDescending(x => x.AppointmentCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppointmentsByDepartmentDto>> GetAppointmentsByDepartmentAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .GroupBy(a => new { a.Doctor.DepartmentId, a.Doctor.Department.Name })
            .Select(g => new AppointmentsByDepartmentDto
            {
                DepartmentId = g.Key.DepartmentId,
                DepartmentName = g.Key.Name,
                AppointmentCount = g.Count()
            })
            .OrderByDescending(x => x.AppointmentCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppointmentsByStatusDto>> GetAppointmentsByStatusAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .GroupBy(a => a.Status)
            .Select(g => new AppointmentsByStatusDto { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DoctorNoAppointmentsTodayDto>> GetDoctorsWithNoAppointmentsTodayAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.Doctors
            .AsNoTracking()
            .Where(d => d.IsActive && !d.Appointments.Any(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow))
            .Select(d => new DoctorNoAppointmentsTodayDto { DoctorId = d.Id, DoctorName = d.FullName, Specialty = d.Specialty })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BusiestDoctorDto>> GetBusiestDoctorsAsync(int top = 5, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .GroupBy(a => new { a.DoctorId, a.Doctor.FullName })
            .Select(g => new BusiestDoctorDto { DoctorId = g.Key.DoctorId, DoctorName = g.Key.FullName, TotalAppointments = g.Count() })
            .OrderByDescending(x => x.TotalAppointments)
            .Take(top)
            .ToListAsync(cancellationToken);
    }
}
