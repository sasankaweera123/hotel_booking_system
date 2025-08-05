namespace HotelBookingApp.Dto;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Location { get; set; } = "";
    public List<RoomDto> Rooms { get; set; } = new();
}