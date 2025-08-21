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
            if (room == null)
                logQueue.Queue.Enqueue(new LogMessage
                {
                    Level = "WARN",
                    Message = $"Room not found for BookingId: {b.Id}, RoomId: {b.RoomId}"
                });
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
                CustomerName = b.CustomerName,
                CustomerNic = b.CustomerNic,
                Status = b.Status,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                SpecialRequests = b.SpecialRequest ?? "None",
            };
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new BookingCreateViewModel
        {
            Hotels = await roomService.GetHotelsAsync(),
            RoomTypes = await roomService.GetRoomTypesAsync()
        };

        if (!User.IsInRole("Admin")) return View(model);
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = "Admin user accessed Booking Create page"
        });
        
        var claim = User.Claims.FirstOrDefault(c => c.Type == "HotelId");
        if (claim == null) return View(model);
        model.SelectedHotelId = int.Parse(claim.Value);
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"Admin user selected HotelId: {model.SelectedHotelId}"
        });
        
        model.Rooms = await roomService.GetAvailableRoomsByHotelAsync(model.SelectedHotelId);

        return View(model);
    }
    
    // GET /Booking/RoomsForHotel?hotelId=1
    [HttpGet]
    public async Task<IActionResult> RoomsForHotel(int hotelId)
    {
        var rooms = await roomService.GetAvailableRoomsByHotelAsync(hotelId);
        var roomTypes = await roomService.GetRoomTypesAsync();
        
        var payload = rooms.Select(r => new {
            id = r.Id,
            roomNumber = r.RoomNumber,
            roomTypeName = roomTypes.FirstOrDefault(rt => rt.Id == r.RoomTypeId)?.Name ?? "Unknown"
        });
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"Fetched {rooms.Count} rooms for HotelId: {hotelId}"
        });

        return Json(payload);
    }


    [HttpPost]
    public async Task<IActionResult> Create(BookingCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "ERROR",
                Message = "Model state is invalid during booking creation"
            });
            
            model.Hotels = await roomService.GetHotelsAsync();
            if (model.SelectedHotelId != 0)
                model.Rooms = await roomService.GetAvailableRoomsByHotelAsync(model.SelectedHotelId);
            return View(model);
        }
        model.Booking.HotelId = model.SelectedHotelId;
        Console.WriteLine($"Creating booking for HotelId: {model.Booking.HotelId}, RoomId: {model.Booking.RoomId}");
        await bookingService.CreateBookingAsync(model.Booking);
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"Booking created successfully for HotelId: {model.Booking.HotelId}, RoomId: {model.Booking.RoomId}"
        });
        
        await roomService.SetRoomAvailabilityAsync(model.Booking.RoomId, false);
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"RoomId: {model.Booking.RoomId} set to unavailable after booking creation"
        });
        
        return RedirectToAction(nameof(Index));
    }


    // GET: /Booking/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var token = HttpContext.Session.GetString("jwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Login");

        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"User accessed Booking Edit page for BookingId: {id}"
        });

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
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"Attempting to update BookingId: {id}"
        });

        if (!ModelState.IsValid) return View(booking);

        Console.WriteLine($"Booking ID: {booking.Id}, Hotel ID: {booking.HotelId}, Room ID: {booking.RoomId}");
        var success = await bookingService.UpdateBookingAsync(id, booking);
        if (success)
        {
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"Booking updated successfully for BookingId: {id}"
            });
            
            TempData["Success"] = "Booking updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "ERROR",
            Message = $"Failed to update BookingId: {id}"
        });

        ViewBag.Error = "Failed to update booking.";

        return View(booking);
    }

    // GET: /Booking/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var token = HttpContext.Session.GetString("jwtToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Login");

        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = $"User accessed Booking Delete page for BookingId: {id}"
        });

        var booking = await bookingService.GetBookingByIdAsync(id);
        
        if (booking == null)
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "WARN",
                Message = $"Booking not found for BookingId: {id}"
            });
            return NotFound();
        }

        return View(booking);
    }

    // POST: /Booking/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Fetch the booking to get the RoomId
        var booking = await bookingService.GetBookingByIdAsync(id);
        if (booking == null)
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "WARN",
                Message = $"Booking not found for BookingId: {id} during deletion"
            });
            TempData["Success"] = "Booking not found.";
            return RedirectToAction(nameof(Index));
        }

        var success = await bookingService.DeleteBookingAsync(id);

        if (success)
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"Booking deleted successfully for BookingId: {id}"
            });
            await roomService.SetRoomAvailabilityAsync(booking.RoomId, true);
            TempData["Success"] = "Booking deleted successfully!";
        }
        else
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "ERROR",
                Message = $"Failed to delete BookingId: {id}"
            });
            TempData["Success"] = "Failed to delete booking.";
        }

        return RedirectToAction(nameof(Index));
    }
}