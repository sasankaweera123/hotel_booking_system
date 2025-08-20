using HotelBookingApp.Dto;

namespace HotelBookingApp.Models;

public class BookingCreateViewModel
{
    public int SelectedHotelId { get; set; }
    public List<HotelDto> Hotels { get; set; } = new();
    public List<RoomDto> Rooms { get; set; } = new();

    public BookingDto Booking { get; set; } = new BookingDto();
}