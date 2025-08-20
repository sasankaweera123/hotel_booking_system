using HotelBookingApp.Dto;

namespace HotelBookingApp.Models;

public class EditRoomViewModel
{
    public RoomDto Room { get; set; } = new();
    public IEnumerable<HotelDto> Hotels { get; set; } = new List<HotelDto>();
    public IEnumerable<RoomTypeDto> RoomTypes { get; set; } = new List<RoomTypeDto>();
}