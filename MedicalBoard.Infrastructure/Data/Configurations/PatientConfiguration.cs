using MedicalBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalBoard.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PatientNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.PatientNumber)
            .IsUnique();

        // Supports "search patients by name or phone" lookups
        builder.HasIndex(p => p.FullName);
        builder.HasIndex(p => p.Phone);
    }
}
