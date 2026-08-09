using MedicalBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalBoard.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.AppointmentDate)
            .IsRequired();

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CreatedByUser)
            .WithMany(u => u.CreatedAppointments)
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent a doctor from double-booking the same slot
        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate })
            .IsUnique();

        // Prevent a patient from double-booking the same slot
        builder.HasIndex(a => new { a.PatientId, a.AppointmentDate })
            .IsUnique();

        // Supports doctor/date/status report queries
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.AppointmentDate);
    }
}
