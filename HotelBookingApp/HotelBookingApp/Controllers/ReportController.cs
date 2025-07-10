using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class ReportController(ReportService reportService) : Controller
{
    public IActionResult Index() => View();
    
    [HttpPost]
    public IActionResult Generate(DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate)
        {
            ViewBag.Alert = "From date must be earlier than to date.";
            return View("Index");
        }
        var reports = reportService.GenerateReports(fromDate, toDate);
        return View("ReportView", reports);
    }
}