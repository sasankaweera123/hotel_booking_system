namespace HotelBookingApp.Dto;

public class RoomDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = "";
    public int RoomTypeId { get; set; }
    public int HotelId { get; set; }
}