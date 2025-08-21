using HotelBookingApp.Dto;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

 public class RoomController(RoomService roomService,  LogQueue logQueue) : Controller
 {
     // GET: /Room
        public async Task<IActionResult> Index()
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = "User accessed Room Index page"
            });
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Login");

            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = "Room Index page loaded successfully"
            });

            var rooms = await roomService.GetRoomsAsync();
            var hotels = await roomService.GetHotelsAsync();
            var roomTypes = await roomService.GetRoomTypesAsync();

            var viewModel = rooms.Select(r => new RoomListViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                RoomTypeName = roomTypes.FirstOrDefault(rt => rt.Id == r.RoomTypeId)?.Name ?? "Unknown",
                HotelName = hotels.FirstOrDefault(h => h.Id == r.HotelId)?.Name ?? "Unknown"
            }).ToList();
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"Room Index page loaded with {viewModel.Count} rooms"
            });

            return View(viewModel);
        }

        // GET: /Room/Create
        public IActionResult Create()
        {
            return View(new RoomDto());
        }

        // POST: /Room/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDto room)
        {
            if (!ModelState.IsValid) return View(room);
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"Creating new room: {room.RoomNumber}, HotelId: {room.HotelId}, RoomTypeId: {room.RoomTypeId}"
            });
            
            var success = await roomService.CreateRoomAsync(room);
            if (success) return RedirectToAction(nameof(Index));
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "ERROR",
                Message = "Failed to create room"
            });
            
            ViewBag.Error = "Failed to create room.";
            return View(room);
        }

        // GET: /Room/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"Editing room with ID: {id}"
            });

            var hotels = await roomService.GetHotelsAsync();
            var roomTypes = await roomService.GetRoomTypesAsync();

            var viewModel = new EditRoomViewModel
            {
                Room = room,
                Hotels = hotels,
                RoomTypes = roomTypes
            };

            return View(viewModel);
        }

// POST: /Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRoomViewModel vm)
        {
            Console.WriteLine($"Edit Room: {id}, ModelState.IsValid: {ModelState.IsValid}");
            if (id != vm.Room.Id) return BadRequest();
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"Updating room with ID: {id}, RoomNumber: {vm.Room.RoomNumber}, HotelId: {vm.Room.HotelId}, RoomTypeId: {vm.Room.RoomTypeId}"
            });

            if (ModelState.IsValid)
            {
                logQueue.Queue.Enqueue(new LogMessage
                {
                    Level = "INFO",
                    Message = $"Attempting to update room with ID: {id}"
                });
                var success = await roomService.UpdateRoomAsync(id, vm.Room);
                if (success) return RedirectToAction(nameof(Index));
                ViewBag.Error = "Failed to update room.";
            }

            // Reload dropdowns if validation fails
            vm.Hotels = await roomService.GetHotelsAsync();
            vm.RoomTypes = await roomService.GetRoomTypesAsync();

            return View(vm);
        }


        // GET: /Room/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"User accessed Room Delete page for RoomId: {id}"
            });
            var room = await roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = $"Room Delete page loaded successfully for RoomId: {id}"
            });
            return View(room);
        }

        // POST: /Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await roomService.DeleteRoomAsync(id);
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = success ? "INFO" : "ERROR",
                Message = success ? $"Room with ID: {id} deleted successfully." : $"Failed to delete room with ID: {id}."
            });
            TempData["Success"] = success ? "Room deleted successfully!" : "Failed to delete room.";
            return RedirectToAction(nameof(Index));
        }
    }