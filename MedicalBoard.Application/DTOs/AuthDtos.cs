using System.ComponentModel.DataAnnotations;

namespace MedicalBoard.Application.DTOs;

public class LoginDto
{
    [Required, StringLength(100)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}

public class AuthenticatedUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? DoctorId { get; set; }
    public int? DepartmentId { get; set; }
    public List<string> Roles { get; set; } = new();
}
