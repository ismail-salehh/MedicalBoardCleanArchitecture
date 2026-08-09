using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

public class DoctorsController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;

    public DoctorsController(IDoctorService doctorService, IDepartmentService departmentService)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(int? departmentId, bool? isActive)
    {
        var doctors = await _doctorService.GetAllAsync(departmentId, isActive);
        ViewBag.Departments = await _departmentService.GetAllAsync();
        ViewBag.SelectedDepartmentId = departmentId;
        ViewBag.SelectedIsActive = isActive;
        return View(doctors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        if (doctor is null) return NotFound();
        return View(doctor);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDepartmentsAsync();
        return View(new CreateDoctorDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDoctorDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsAsync();
            return View(dto);
        }

        var result = await _doctorService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await PopulateDepartmentsAsync();
            return View(dto);
        }

        TempData["Success"] = "Doctor created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        if (doctor is null) return NotFound();

        await PopulateDepartmentsAsync();
        return View(new UpdateDoctorDto
        {
            Id = doctor.Id,
            EmployeeNumber = doctor.EmployeeNumber,
            FullName = doctor.FullName,
            Specialty = doctor.Specialty,
            Phone = doctor.Phone,
            Email = doctor.Email,
            DepartmentId = doctor.DepartmentId,
            IsActive = doctor.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateDoctorDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsAsync();
            return View(dto);
        }

        var result = await _doctorService.UpdateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await PopulateDepartmentsAsync();
            return View(dto);
        }

        TempData["Success"] = "Doctor updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id, bool isActive)
    {
        var result = await _doctorService.SetActiveStatusAsync(id, isActive);
        TempData[result.Success ? "Success" : "Error"] =
            result.Success ? "Doctor status updated." : result.Error;
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDepartmentsAsync()
    {
        ViewBag.Departments = (await _departmentService.GetAllAsync()).Where(d => d.IsActive).ToList();
    }
}
