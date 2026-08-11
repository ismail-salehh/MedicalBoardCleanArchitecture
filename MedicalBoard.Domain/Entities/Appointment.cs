// UPDATED VERSION of your existing Appointment.cs -- adds the Cancel()/Confirm()/Complete()
// domain-rule methods called out in the spec. Replace your current file with this one
// (or copy just the three methods in).
using MedicalBoard.Domain.Enums;
using MedicalBoard.Domain.Exceptions;

namespace MedicalBoard.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string? Notes { get; set; }

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new DomainRuleViolationException("Only a pending appointment can be confirmed.");

        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == AppointmentStatus.Completed)
            throw new DomainRuleViolationException("A completed appointment cannot be cancelled.");
        if (Status == AppointmentStatus.Cancelled)
            throw new DomainRuleViolationException("Appointment is already cancelled.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("A cancellation reason is required.");

        Status = AppointmentStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new DomainRuleViolationException("A cancelled appointment cannot be completed.");
        if (Status != AppointmentStatus.Confirmed)
            throw new DomainRuleViolationException("Only a confirmed appointment can be completed.");

        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}
