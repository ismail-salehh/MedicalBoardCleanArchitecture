using MedicalBoard.Domain.Enums;

namespace MedicalBoard.Application.DTOs;

public class AppointmentsByDoctorDto
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
}

public class AppointmentsByDepartmentDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
}

public class AppointmentsByStatusDto
{
    public AppointmentStatus Status { get; set; }
    public int Count { get; set; }
}

public class DoctorNoAppointmentsTodayDto
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}

public class BusiestDoctorDto
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int TotalAppointments { get; set; }
}
