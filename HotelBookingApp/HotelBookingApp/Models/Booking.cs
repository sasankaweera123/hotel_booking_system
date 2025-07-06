using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Models;

public class Booking
{
    public int Id { get; set; }
    public String CustomerName { get; set; }
    public String CustomerNIC { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int NumberOfGuests { get; set; }
    public List<RoomType> RoomTypes { get; set; } = new();
    public string? SpecialRequest { get; set; }
    public bool IsRecurring { get; set; }
}