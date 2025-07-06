using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class ReportController(ReportService reportService) : Controller
{
    public IActionResult Index() => View();
    
    [HttpPost]
    public IActionResult Generate(DateTime fromDate, DateTime toDate)
    {
        var reports = reportService.GenerateReports(fromDate, toDate);
        return View("ReportView", reports);
    }
}