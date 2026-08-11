using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

[Authorize]
public class PatientsController : Controller
{
    private readonly IPatientService _patientService;
    public PatientsController(IPatientService patientService) => _patientService = patientService;

    [Authorize(Policy = PermissionCodes.PatientView)]
    public async Task<IActionResult> Index(string? term)
    {
        ViewData["Term"] = term;
        var patients = string.IsNullOrWhiteSpace(term)
            ? await _patientService.GetAllAsync()
            : await _patientService.SearchAsync(term);

        return View(patients);
    }

    [Authorize(Policy = PermissionCodes.PatientView)]
    public async Task<IActionResult> Details(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null) return NotFound();
        return View(patient);
    }

    [Authorize(Policy = PermissionCodes.PatientCreate)]
    public IActionResult Create() => View(new CreatePatientDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.PatientCreate)]
    public async Task<IActionResult> Create(CreatePatientDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _patientService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create patient.");
            return View(dto);
        }

        TempData["Success"] = "Patient created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = PermissionCodes.PatientEdit)]
    public async Task<IActionResult> Edit(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null) return NotFound();

        return View(new UpdatePatientDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            NationalIdentifier = patient.NationalIdentifier,
            DateOfBirth = patient.DateOfBirth,
            Phone = patient.Phone,
            Email = patient.Email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PermissionCodes.PatientEdit)]
    public async Task<IActionResult> Edit(UpdatePatientDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _patientService.UpdateAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to update patient.");
            return View(dto);
        }

        TempData["Success"] = "Patient updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id, bool isActive)
    {
        await _patientService.SetActiveStatusAsync(id, isActive);
        return RedirectToAction(nameof(Index));
    }
}
