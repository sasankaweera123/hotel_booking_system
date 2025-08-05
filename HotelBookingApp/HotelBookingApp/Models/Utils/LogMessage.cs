namespace HotelBookingApp.Models.Utils;

public class LogMessage
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Level { get; set; } = "INFO"; // INFO, WARN, ERROR
    public string Message { get; set; } = "";
}