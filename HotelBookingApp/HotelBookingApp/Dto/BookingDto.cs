namespace HotelBookingApp.Dto;

public class BookingDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public int RoomId { get; set; }
    public string Status { get; set; } = "";
    public string Username { get; set; } = "";
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerNic { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; } = DateTime.Now;
    public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(1);
    public string? SpecialRequest { get; set; }
}