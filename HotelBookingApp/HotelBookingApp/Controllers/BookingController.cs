using HotelBookingApp.Models;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class BookingController : Controller
{
    private readonly BookingService _bookingService;
    private readonly RequestService _requestService;

    public BookingController(BookingService bookingService, RequestService requestService)
    {
        _bookingService = bookingService;
        _requestService = requestService;
    }

    public IActionResult Index() => View(_bookingService.GetAll());

    public IActionResult Create() => View();
    [HttpPost]
    public IActionResult Create(Booking booking) { _bookingService.Add(booking); return RedirectToAction("Index"); }

    public IActionResult Edit(int id) => View(_bookingService.GetById(id));
    [HttpPost]
    public IActionResult Edit(Booking booking) { _bookingService.Update(booking); return RedirectToAction("Index"); }

    public IActionResult Delete(int id) => View(_bookingService.GetById(id));
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id) { _bookingService.Delete(id); return RedirectToAction("Index"); }

    public IActionResult WeeklyReport()
    {
        var bookings = _bookingService.GetAll();
        var requests = _requestService.GetAll();

        var result = bookings
            .GroupBy(b => b.CheckIn.DayOfWeek)
            .ToDictionary(g => g.Key.ToString(), g => new
            {
                Bookings = g.ToList(),
                Requests = requests.Where(r => r.Date.DayOfWeek == g.Key).ToList()
            });

        return View(result);
    }

    [HttpPost]
    public JsonResult Predict(string userMessage)
    {
        string response = userMessage.ToLower() switch
        {
            string s when s.Contains("availability") => "Rooms are usually available on weekdays.",
            string s when s.Contains("price") => "Prices tend to rise on weekends due to high demand.",
            _ => "I'm sorry, I can't process that request."
        };
        return Json(new { reply = response });
    }
}