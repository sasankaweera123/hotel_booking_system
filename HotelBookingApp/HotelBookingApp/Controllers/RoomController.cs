using HotelBookingApp.Dto;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

 public class RoomController : Controller
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: /Room
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Login");

            var rooms = await _roomService.GetRoomsAsync();
            var hotels = await _roomService.GetHotelsAsync();
            var roomTypes = await _roomService.GetRoomTypesAsync();

            var viewModel = rooms.Select(r => new RoomListViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                RoomTypeName = roomTypes.FirstOrDefault(rt => rt.Id == r.RoomTypeId)?.Name ?? "Unknown",
                HotelName = hotels.FirstOrDefault(h => h.Id == r.HotelId)?.Name ?? "Unknown"
            }).ToList();

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
            if (ModelState.IsValid)
            {
                var success = await _roomService.CreateRoomAsync(room);
                if (success) return RedirectToAction(nameof(Index));
                ViewBag.Error = "Failed to create room.";
            }
            return View(room);
        }

        // GET: /Room/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();

            var hotels = await _roomService.GetHotelsAsync();
            var roomTypes = await _roomService.GetRoomTypesAsync();

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

            if (ModelState.IsValid)
            {
                var success = await _roomService.UpdateRoomAsync(id, vm.Room);
                if (success) return RedirectToAction(nameof(Index));
                ViewBag.Error = "Failed to update room.";
            }

            // Reload dropdowns if validation fails
            vm.Hotels = await _roomService.GetHotelsAsync();
            vm.RoomTypes = await _roomService.GetRoomTypesAsync();

            return View(vm);
        }


        // GET: /Room/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // POST: /Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _roomService.DeleteRoomAsync(id);
            TempData["Success"] = success ? "Room deleted successfully!" : "Failed to delete room.";
            return RedirectToAction(nameof(Index));
        }
    }