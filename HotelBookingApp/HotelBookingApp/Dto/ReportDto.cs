namespace HotelBookingApp.Dto;

public class ReportDto
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = "";
    public int TotalBookings { get; set; }
    public int ActiveBookings { get; set; }
    public int CancelledBookings { get; set; }
}