using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Models;

public class Booking
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerNic { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int NumberOfGuests { get; set; }
    public required List<BookingRoomType> RoomTypes { get; set; } = new();
    public string? SpecialRequest { get; set; }
    public bool IsRecurring { get; set; }
    
    public override string ToString()
    {
        return $"Booking [Id={Id}, CustomerName={CustomerName}, CustomerNic={CustomerNic}, CheckIn={CheckIn}, CheckOut={CheckOut}, NumberOfGuests={NumberOfGuests}, RoomTypes={string.Join(",", RoomTypes)}, SpecialRequest={SpecialRequest}, IsRecurring={IsRecurring}]";
    }
}