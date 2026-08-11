using System.Security.Claims;
using MedicalBoard.Application.DTOs;
using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;

    public AppointmentsController(
        IAppointmentService appointmentService,
        IDoctorService doctorService,
        IPatientService patientService)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _patientService = patientService;
    }

    public async Task<IActionResult> Index(int? doctorId, int? patientId, AppointmentStatus? status)
    {
        var appointments = await _appointmentService.GetAllAsync(doctorId, patientId, status);

        ViewBag.Doctors = await _doctorService.GetAllAsync();
        ViewBag.Patients = await _patientService.SearchAsync(string.Empty);
        ViewBag.SelectedDoctorId = doctorId;
        ViewBag.SelectedPatientId = patientId;
        ViewBag.SelectedStatus = status;
        ViewBag.StatusCounts = await _appointmentService.GetCountsByStatusAsync();

        return View(appointments);
    }

    public async Task<IActionResult> Details(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment is null) return NotFound();
        return View(appointment);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateAppointmentDto
        {
             AppointmentDate = DateTime.Now.AddHours(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        dto.CreatedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(dto);
        }

        var result = await _appointmentService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await PopulateDropdownsAsync();
            return View(dto);
        }

        TempData["Succeeded"] = "Appointment created.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id)
    {
        var result = await _appointmentService.ConfirmAsync(id);
        TempData[result.Succeeded ? "Succeeded" : "Error"] =
            result.Succeeded ? "Appointment confirmed." : result.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var result = await _appointmentService.CompleteAsync(id);
        TempData[result.Succeeded ? "Succeeded" : "Error"] =
            result.Succeeded ? "Appointment marked complete." : result.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Cancel(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment is null) return NotFound();

        return View(new CancelAppointmentDto { Id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(CancelAppointmentDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _appointmentService.CancelAsync(dto);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(dto);
        }

        TempData["Succeeded"] = "Appointment cancelled.";
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    private async Task PopulateDropdownsAsync()
    {
        ViewBag.Doctors = await _doctorService.GetActiveDoctorsAsync();
        ViewBag.Patients = (await _patientService.SearchAsync(string.Empty)).Where(p => p.IsActive).ToList();
    }
}
