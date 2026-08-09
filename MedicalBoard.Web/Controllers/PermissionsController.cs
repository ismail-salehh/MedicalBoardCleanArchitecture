using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

[Authorize(Policy = PermissionCodes.RoleView)]
public class PermissionsController : Controller
{
    private readonly IPermissionService _permissionService;
    public PermissionsController(IPermissionService permissionService) => _permissionService = permissionService;

    public async Task<IActionResult> Index()
    {
        var permissions = await _permissionService.GetAllAsync();
        return View(permissions);
    }
}
