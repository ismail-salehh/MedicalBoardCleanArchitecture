using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

public class PatientsController : Controller
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var patients = await _patientService.SearchAsync(search);
        ViewBag.SearchTerm = search;
        return View(patients);
    }

    public async Task<IActionResult> Details(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null) return NotFound();
        return View(patient);
    }

    public IActionResult Create()
    {
        return View(new CreatePatientDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePatientDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _patientService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(dto);
        }

        TempData["Success"] = "Patient created.";
        return RedirectToAction(nameof(Index));
    }

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
            Email = patient.Email,
            IsActive = patient.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdatePatientDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);

        var result = await _patientService.UpdateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(dto);
        }

        TempData["Success"] = "Patient updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id, bool isActive)
    {
        var result = await _patientService.SetActiveStatusAsync(id, isActive);
        TempData[result.Success ? "Success" : "Error"] =
            result.Success ? "Patient status updated." : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
