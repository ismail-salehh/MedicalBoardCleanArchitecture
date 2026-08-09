namespace MedicalBoard.Application.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    int? DoctorId { get; }
    int? DepartmentId { get; }
    bool IsAuthenticated { get; }
}
