using System.ComponentModel.DataAnnotations;

namespace MedicalBoard.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class AuthenticatedUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? DoctorId { get; set; }
    public int? DepartmentId { get; set; }
    public List<string> Roles { get; set; } = new();
}