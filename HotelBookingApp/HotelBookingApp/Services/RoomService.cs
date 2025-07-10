using HotelBookingApp.Models;
using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Services;

public class RoomService
{
    private readonly List<Room> _rooms = new();

    public RoomService()
    {
        _rooms.Add(new Room { Id = 1, Type = RoomType.DOUBLE, Price = 100, IsAvailable = false }); // For Naruto
        _rooms.Add(new Room { Id = 2, Type = RoomType.SUITE, Price = 200, IsAvailable = false });  // For Sakura
        _rooms.Add(new Room { Id = 3, Type = RoomType.DOUBLE, Price = 100, IsAvailable = false }); // For Sakura
        _rooms.Add(new Room { Id = 4, Type = RoomType.SINGLE, Price = 80, IsAvailable = false });  // For Sasuke

        // Add more rooms for diversity/testing
        _rooms.Add(new Room { Id = 5, Type = RoomType.SINGLE, Price = 80, IsAvailable = true });
        _rooms.Add(new Room { Id = 6, Type = RoomType.DOUBLE, Price = 100, IsAvailable = true });
        _rooms.Add(new Room { Id = 7, Type = RoomType.SUITE, Price = 200, IsAvailable = true });
        _rooms.Add(new Room { Id = 8, Type = RoomType.SUITE, Price = 220, IsAvailable = false });
    }

    public List<Room> GetAll() => _rooms;
    public Room? GetById(int id) => _rooms.FirstOrDefault(r => r.Id == id);
    public List<Room> GetAvailableRooms(RoomType type)
        => _rooms.Where(r => r.Type == type && r.IsAvailable).ToList();
    public void Add(Room room) => _rooms.Add(room);
    public void Update(Room room)
    {
        var index = _rooms.FindIndex(r => r.Id == room.Id);
        if (index != -1)
            _rooms[index] = room;
    }
    
    public void UpdateAvailability(int id, bool isAvailable)
    {
        var room = GetById(id);
        if (room == null) return;
        room.IsAvailable = isAvailable;
        Update(room);
    }
    
    public void Delete(int id) => _rooms.RemoveAll(r => r.Id == id);
}