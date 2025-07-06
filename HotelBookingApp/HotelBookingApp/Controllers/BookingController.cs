using HotelBookingApp.Models;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class BookingController(BookingService bookingService) : Controller
{
    public IActionResult Index() => View(bookingService.GetAll());

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Booking booking)
    {
        var bookingId = bookingService.Add(booking); 
        if(bookingId > 0) return RedirectToAction("Index");
        else
        {
            ViewBag.Alert = "Booking creation failed. Room might not be available or invalid data provided.";
            return View(booking);
        }
    }

    public IActionResult Edit(int id) => View(bookingService.GetById(id));
    [HttpPost]
    public IActionResult Edit(Booking booking) { bookingService.Update(booking); return RedirectToAction("Index"); }

    public IActionResult Delete(int id) => View(bookingService.GetById(id));
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id) { bookingService.Delete(id); return RedirectToAction("Index"); }

    [HttpPost]
    public JsonResult Predict(string userMessage)
    {
        var response = userMessage.ToLower() switch
        {
            var s when s.Contains("availability") => "Rooms are usually available on weekdays.",
            var s when s.Contains("price") => "Prices tend to rise on weekends due to high demand.",
            _ => "I'm sorry, I can't process that request."
        };
        return Json(new { reply = response });
    }
}