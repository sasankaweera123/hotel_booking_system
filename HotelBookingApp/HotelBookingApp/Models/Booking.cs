namespace HotelBookingApp.Models;

public class Booking
{
    public int Id { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string RoomType { get; set; }
    public string SpecialRequest { get; set; }
    public bool IsRecurring { get; set; }
}