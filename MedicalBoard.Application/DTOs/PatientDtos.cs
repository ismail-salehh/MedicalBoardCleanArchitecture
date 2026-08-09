using System.ComponentModel.DataAnnotations;

namespace MedicalBoard.Application.DTOs;

public class PatientDto
{
    public int Id { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? NationalIdentifier { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePatientDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? NationalIdentifier { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }
}

public class UpdatePatientDto
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? NationalIdentifier { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    public bool IsActive { get; set; }
}
