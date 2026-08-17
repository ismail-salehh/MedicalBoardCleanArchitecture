namespace MedicalBoard.Domain.Entities;

public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public IList<Doctor> Doctors { get; set; } = new List<Doctor>();
    public IEnumerable<User> Users { get; set; } = new List<User>();
}
