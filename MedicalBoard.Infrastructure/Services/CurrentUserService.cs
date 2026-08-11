using System.Security.Claims;
using MedicalBoard.Application.Interfaces;

namespace MedicalBoard.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public int? UserId => int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public string? Username => User?.Identity?.Name;

    public int? DoctorId => int.TryParse(User?.FindFirstValue("DoctorId"), out var id) ? id : null;

    public int? DepartmentId => int.TryParse(User?.FindFirstValue("DepartmentId"), out var id) ? id : null;
}
