using System.ComponentModel.DataAnnotations;

namespace MedicalBoard.Application.DTOs;

public class RoleListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int UserCount { get; set; }
}

public class RoleDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}

public class SaveRoleDto
{
    public int? Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    public List<int> PermissionIds { get; set; } = new();
}
