using HotelBookingApp.Dto;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class ReportController : Controller
{
    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var token = HttpContext.Session.GetString("jwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Login");

        return View(new List<ReportDto>()); // Empty list initially
    }

    [HttpPost]
    public async Task<IActionResult> Generate(DateTime fromDate, DateTime toDate)
    {
        var token = HttpContext.Session.GetString("jwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Login");

        var reports = await _reportService.GetBookingSummaryAsync(fromDate, toDate);
        ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");

        return View("Index", reports);
    }
}