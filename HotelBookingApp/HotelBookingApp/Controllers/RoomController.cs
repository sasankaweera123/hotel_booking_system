using HotelBookingApp.Models;
using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class RoomController(RoomService roomService): Controller
{
    
    public IActionResult Index() => View(roomService.GetAll());
    public IActionResult Create() => View();
    [HttpPost]
    public IActionResult Create(Room room)
    {
        if (!ModelState.IsValid)
        {
            return View(room);
        }
        roomService.Add(room);
        return RedirectToAction("Index");
    }
    
    public IActionResult Edit(int id) => View(roomService.GetById(id));
    
    [HttpPost]
    public IActionResult Edit(Room room)
    {
        
        if (!ModelState.IsValid)
        {
            return View(room);
        }
        roomService.Update(room);
        return RedirectToAction("Index");
    }
    
    public IActionResult Delete(int id) => View(roomService.GetById(id));
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        roomService.Delete(id);
        return RedirectToAction("Index");
    }
    
    [HttpGet, ActionName("GetAvailableRooms")]
    public JsonResult GetAvailableRooms(RoomType roomType)
    {
        var availableRooms = roomService.GetAvailableRooms(roomType);
        return Json(availableRooms);
    }
    
    
    
}