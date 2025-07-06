namespace HotelBookingApp.Models;

public class SpecialRequest
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public required string Request { get; set; }
    public DateTime Date { get; set; }
}