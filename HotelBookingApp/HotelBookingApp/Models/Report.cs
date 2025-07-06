namespace HotelBookingApp.Models;

public class Report
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalRoomsBooked { get; set; }
    public string? Comments { get; set; }
}