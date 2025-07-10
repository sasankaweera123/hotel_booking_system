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
        Console.WriteLine($"Creating booking for {booking}");
        
        if(booking.RoomTypes.Count <= 0)
        {
            ViewBag.Alert = "Invalid room selected. Please choose a valid room.";
            return View(booking);
        }
        if(booking.CheckIn >= booking.CheckOut)
        {
            ViewBag.Alert = "Check-out date must be after check-in date.";
            return View(booking);
        }
        
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
    public IActionResult Edit(Booking booking)
    {
        
        if (!ModelState.IsValid)
        {
            ViewBag.Alert = "Invalid booking data. Please check your input.";
            return View(booking);
        }
        
        if (booking.RoomTypes.Count <= 0)
        {
            ViewBag.Alert = "Invalid room selected. Please choose a valid room.";
            return View(booking);
        }
        
        if (booking.CheckIn >= booking.CheckOut)
        {
            ViewBag.Alert = "Check-out date must be after check-in date.";
            return View(booking);
        }
        
        bookingService.Update(booking); 
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id) => View(bookingService.GetById(id));
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id) { bookingService.Delete(id); return RedirectToAction("Index"); }
}