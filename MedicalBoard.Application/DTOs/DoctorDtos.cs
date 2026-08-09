using System.ComponentModel.DataAnnotations;

namespace MedicalBoard.Application.DTOs;

public class DoctorDto
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateDoctorDto
{
    [Required, StringLength(30)]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Specialty { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please select a department.")]
    public int DepartmentId { get; set; }
}

public class UpdateDoctorDto
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Specialty { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please select a department.")]
    public int DepartmentId { get; set; }

    public bool IsActive { get; set; }
}
