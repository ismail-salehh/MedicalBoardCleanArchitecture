
using System.ComponentModel.DataAnnotations;

namespace MedicalBoard.Application.DTOs;

public class UserListItemDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class UserDetailDto : UserListItemDto
{
    public int? DoctorId { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<int> RoleIds { get; set; } = new();
}

public class CreateUserDto
{
    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    public int? DoctorId { get; set; }
    public int? DepartmentId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}

public class UpdateUserDto
{
    public int Id { get; set; }

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    public int? DoctorId { get; set; }
    public int? DepartmentId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}