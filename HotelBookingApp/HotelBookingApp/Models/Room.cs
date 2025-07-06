using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Models;

public class Room
{
    public int Id { get; set; }
    public RoomType Type { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}