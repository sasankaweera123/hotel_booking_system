using HotelBookingApp.Models;

namespace HotelBookingApp.Services;

public class BookingService
{
    private readonly List<Booking> _bookings = new();

    public BookingService()
    {
        _bookings.Add(new Booking
        {
            Id = 1,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(2),
            RoomType = "Standard",
            SpecialRequest = "Late check-in",
            IsRecurring = false
        });
    }

    public List<Booking> GetAll() => _bookings;
    public Booking? GetById(int id) => _bookings.FirstOrDefault(b => b.Id == id);
    public void Add(Booking booking) => _bookings.Add(booking);
    public void Update(Booking booking) { Delete(booking.Id); _bookings.Add(booking); }
    public void Delete(int id) => _bookings.RemoveAll(b => b.Id == id);
}