using MedicalBoard.Application.Interfaces;
using MedicalBoard.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBoard.Web.Controllers;

[Authorize(Policy = PermissionCodes.ReportView)]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    public ReportsController(IReportService reportService) => _reportService = reportService;

    public async Task<IActionResult> Index()
    {
        ViewBag.ByDoctor = await _reportService.GetAppointmentsByDoctorAsync();
        ViewBag.ByDepartment = await _reportService.GetAppointmentsByDepartmentAsync();
        ViewBag.ByStatus = await _reportService.GetAppointmentsByStatusAsync();
        ViewBag.NoAppointmentsToday = await _reportService.GetDoctorsWithNoAppointmentsTodayAsync();
        ViewBag.Busiest = await _reportService.GetBusiestDoctorsAsync();
        return View();
    }
}
