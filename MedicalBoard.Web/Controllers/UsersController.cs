using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public UsersController(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    [Authorize(Policy = PermissionCodes.UserView)]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    [Authorize(Policy = PermissionCodes.UserCreate)]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _roleService.GetAllAsync();
        return View(new CreateUserDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.UserCreate)]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _roleService.GetAllAsync();
            return View(dto);
        }

        var result = await _userService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create user.");
            ViewBag.Roles = await _roleService.GetAllAsync();
            return View(dto);
        }

        TempData["Success"] = "User created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = PermissionCodes.UserEdit)]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();

        ViewBag.Roles = await _roleService.GetAllAsync();
        return View(new UpdateUserDto
        {
            Id = user.Id,
            Email = user.Email,
            DoctorId = user.DoctorId,
            DepartmentId = user.DepartmentId,
            RoleIds = user.RoleIds
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.UserEdit)]
    public async Task<IActionResult> Edit(UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _roleService.GetAllAsync();
            return View(dto);
        }

        var result = await _userService.UpdateAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to update user.");
            ViewBag.Roles = await _roleService.GetAllAsync();
            return View(dto);
        }

        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.UserDeactivate)]
    public async Task<IActionResult> ToggleActive(int id, bool isActive)
    {
        await _userService.SetActiveStatusAsync(id, isActive);
        return RedirectToAction(nameof(Index));
    }
}
