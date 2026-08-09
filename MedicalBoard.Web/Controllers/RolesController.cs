using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

[Authorize]
public class RolesController : Controller
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;

    public RolesController(IRoleService roleService, IPermissionService permissionService)
    {
        _roleService = roleService;
        _permissionService = permissionService;
    }

    [Authorize(Policy = PermissionCodes.RoleView)]
    public async Task<IActionResult> Index()
    {
        var roles = await _roleService.GetAllAsync();
        return View(roles);
    }

    [Authorize(Policy = PermissionCodes.RoleEdit)]
    public async Task<IActionResult> Create()
    {
        ViewBag.Permissions = await _permissionService.GetAllAsync();
        return View("Edit", new SaveRoleDto());
    }

    [Authorize(Policy = PermissionCodes.RoleEdit)]
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role is null) return NotFound();

        ViewBag.Permissions = await _permissionService.GetAllAsync();
        return View(new SaveRoleDto { Id = role.Id, Name = role.Name, Description = role.Description, PermissionIds = role.PermissionIds });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.RoleEdit)]
    public async Task<IActionResult> Save(SaveRoleDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Permissions = await _permissionService.GetAllAsync();
            return View("Edit", dto);
        }

        var result = await _roleService.SaveAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to save role.");
            ViewBag.Permissions = await _permissionService.GetAllAsync();
            return View("Edit", dto);
        }

        TempData["Success"] = "Role saved successfully.";
        return RedirectToAction(nameof(Index));
    }
}
