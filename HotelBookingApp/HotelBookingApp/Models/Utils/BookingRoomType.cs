namespace HotelBookingApp.Models.Utils;

public class BookingRoomType
{
    public int Id { get; set; }
    public RoomType RoomType { get; set; }
    
    public override string ToString()
    {
        return $"BookingRoomType: Id={Id}, RoomType={RoomType}";
    }
}