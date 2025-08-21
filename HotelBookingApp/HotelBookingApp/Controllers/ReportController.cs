using HotelBookingApp.Dto;
using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class ReportController(ReportService reportService, LogQueue logQueue) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = "User accessed Report Index page"
        });
        var token = HttpContext.Session.GetString("jwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Login");

        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = "Report Index page loaded successfully"
        });
        return View(new List<ReportDto>()); // Empty list initially
    }

    [HttpPost]
    public async Task<IActionResult> Generate(DateTime fromDate, DateTime toDate)
    {
        var token = HttpContext.Session.GetString("jwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Login");

        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"Generating booking summary report from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}"
        });
        
        if (fromDate > toDate)
        {
            ModelState.AddModelError("", "From date cannot be later than To date.");
            return View("Index", new List<ReportDto>());
        }
        var reports = await reportService.GetBookingSummaryAsync(fromDate, toDate);
        ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"Booking summary report generated with {reports.Count} entries"
        });

        return View("Index", reports);
    }
}