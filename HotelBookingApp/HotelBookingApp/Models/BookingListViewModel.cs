namespace HotelBookingApp.Models;

public class BookingListViewModel
{
    public int Id { get; set; }
    public string HotelName { get; set; } = "";
    public string RoomNumber { get; set; } = "";
    public string RoomTypeName { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string SpecialRequests { get; set; } = "";
}