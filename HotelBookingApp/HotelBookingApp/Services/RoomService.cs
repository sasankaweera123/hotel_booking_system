using HotelBookingApp.Models;

namespace HotelBookingApp.Services;

public class RoomService
{
    private readonly List<Room> _rooms = new();

    public RoomService()
    {
        _rooms.Add(new Room { Id = 1, Type = "Standard", Price = 100, IsAvailable = true });
        _rooms.Add(new Room { Id = 2, Type = "Deluxe", Price = 200, IsAvailable = true });
    }

    public List<Room> GetAll() => _rooms;
    public Room? GetById(int id) => _rooms.FirstOrDefault(r => r.Id == id);
    public void Add(Room room) => _rooms.Add(room);
    public void Update(Room room) { Delete(room.Id); _rooms.Add(room); }
    public void Delete(int id) => _rooms.RemoveAll(r => r.Id == id);
}