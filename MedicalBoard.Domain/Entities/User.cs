namespace MedicalBoard.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional relationship: this user IS a doctor (login-enabled doctor)
    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    // Optional relationship: this user belongs to a department (e.g. receptionist desk)
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
}
