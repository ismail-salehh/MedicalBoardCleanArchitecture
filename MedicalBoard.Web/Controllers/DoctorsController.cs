using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

[Authorize]
public class DoctorsController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;


    public DoctorsController(IDoctorService doctorService, IDepartmentService departmentService)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
    }

    [Authorize(Policy = PermissionCodes.DoctorView)]
    public async Task<IActionResult> Index(bool includeInactive = false)
    {
        var doctors = await _doctorService.GetAllAsync(includeInactive);
        return View(doctors);
    }

    [Authorize(Policy = PermissionCodes.DoctorView)]
    public async Task<IActionResult> Details(int id)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        if (doctor is null) return NotFound();
        return View(doctor);
    }

    [Authorize(Policy = PermissionCodes.DoctorCreate)]
    public IActionResult Create() => View(new CreateDoctorDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.DoctorCreate)]
    public async Task<IActionResult> Create(CreateDoctorDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _doctorService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create doctor.");
            return View(dto);
        }

        TempData["Success"] = "Doctor created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = PermissionCodes.DoctorEdit)]
    public async Task<IActionResult> Edit(int id)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        if (doctor is null) return NotFound();

        return View(new UpdateDoctorDto
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            Specialty = doctor.Specialty,
            Phone = doctor.Phone,
            Email = doctor.Email,
            DepartmentId = doctor.DepartmentId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.DoctorEdit)]
    public async Task<IActionResult> Edit(UpdateDoctorDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _doctorService.UpdateAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to update doctor.");
            return View(dto);
        }

        TempData["Success"] = "Doctor updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.DoctorDeactivate)]
    public async Task<IActionResult> ToggleActive(int id, bool isActive)
    {
        await _doctorService.SetActiveStatusAsync(id, isActive);
        return RedirectToAction(nameof(Index));
    }
}
