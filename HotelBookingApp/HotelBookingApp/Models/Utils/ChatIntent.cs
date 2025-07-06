namespace HotelBookingApp.Models.Utils;

public class ChatIntent
{
    public string Intent { get; set; }
    public List<string> Patterns { get; set; }
    public string Response { get; set; }
}