using HotelBookingApp.Dto;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

 public class BookingController(BookingService bookingService, RoomService roomService, LogQueue logQueue) : Controller
 {
     // GET: /Booking
     public async Task<IActionResult> Index()
     {
         
         logQueue.Queue.Enqueue(new LogMessage
         {
             Level = "INFO",
             Message = "User accessed Booking Index page"
         });
         
         var token = HttpContext.Session.GetString("jwtToken");
         if (string.IsNullOrEmpty(token))
             return RedirectToAction("Index", "Login");

         var bookings = await bookingService.GetMyBookingsAsync();
         var hotels = await roomService.GetHotelsAsync();
         var rooms = await roomService.GetRoomsAsync();
         var roomTypes = await roomService.GetRoomTypesAsync();

         var viewModel = bookings.Select(b =>
         {
             var room = rooms.FirstOrDefault(r => r.Id == b.RoomId);
             var hotel = hotels.FirstOrDefault(h => h.Id == b.HotelId);
             var roomTypeName = room != null
                 ? (roomTypes.FirstOrDefault(rt => rt.Id == room.RoomTypeId)?.Name ?? "Unknown")
                 : "Unknown";

             return new BookingListViewModel
             {
                 Id = b.Id,
                 HotelName = hotel?.Name ?? "Unknown",
                 RoomNumber = room?.RoomNumber ?? "Unknown",
                 RoomTypeName = roomTypeName,
                 Status = b.Status,
                 CheckInDate = b.CheckInDate,
                 CheckOutDate = b.CheckOutDate,
                 SpecialRequests = b.SpecialRequest ?? "None",
             };
         }).ToList();

         return View(viewModel);
     }

        // GET: /Booking/Create
        public IActionResult Create()
        {
            // Ensure user is logged in
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Login");

            return View(new BookingDto
            {
                Status = "Confirmed",
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            });
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingDto booking)
        {
            if (!ModelState.IsValid) return View(booking);
            
            var success = await bookingService.CreateBookingAsync(booking);
            if (success)
            {
                TempData["Success"] = "Booking created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "Failed to create booking.";
            return View(booking);
        }

        // GET: /Booking/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Login");

            var booking = await bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: /Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingDto booking)
        {
            if (id != booking.Id) return BadRequest();

            if (!ModelState.IsValid) return View(booking);
            
            var success = await bookingService.UpdateBookingAsync(id, booking);
            if (success)
            {
                TempData["Success"] = "Booking updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "Failed to update booking.";

            return View(booking);
        }

        // GET: /Booking/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Login");

            var booking = await bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            return View(booking); // Must match Views/Booking/Delete.cshtml
        }

        // POST: /Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await bookingService.DeleteBookingAsync(id);

            TempData["Success"] = success 
                ? "Booking deleted successfully!" 
                : "Failed to delete booking.";

            return RedirectToAction(nameof(Index));
        }
    }