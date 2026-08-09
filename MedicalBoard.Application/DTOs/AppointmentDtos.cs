using System.ComponentModel.DataAnnotations;
using MedicalBoard.Domain.Enums;

namespace MedicalBoard.Application.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CancellationReason { get; set; }
}

public class CreateAppointmentDto
{
    [Required(ErrorMessage = "Please select a doctor.")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Please select a patient.")]
    public int PatientId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    // TODO: replace with the authenticated user's id once login/claims exist (Milestone 6).
    public int CreatedByUserId { get; set; }
}

public class CancelAppointmentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A cancellation reason is required.")]
    [StringLength(500)]
    public string CancellationReason { get; set; } = string.Empty;
}
