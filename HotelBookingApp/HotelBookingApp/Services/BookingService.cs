using HotelBookingApp.Models;
using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Services;

public class BookingService
{
    private readonly List<Booking> _bookings = new();
    private readonly RequestService _requestService = new();
    private readonly RoomService _roomService = new();

    public BookingService()
    {
        _bookings.Add(new Booking
        {
            Id = 1,
            CustomerName = "Naruto Uzumaki",
            CustomerNIC = "123456789V",
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(2),
            NumberOfGuests = 2,
            RoomTypes = [RoomType.DOUBLE],
            SpecialRequest = "Late check-in",
            IsRecurring = false
        });
        
        _bookings.Add(new Booking
        {
            Id = 2,
            CustomerName = "Sakura Haruno",
            CustomerNIC = "987654321V",
            CheckIn = DateTime.Today.AddDays(3),
            CheckOut = DateTime.Today.AddDays(5),
            NumberOfGuests = 10,
            RoomTypes = [RoomType.SUITE, RoomType.DOUBLE],
            SpecialRequest = "Ocean view",
            IsRecurring = true
        });
        
        _bookings.Add(new Booking
        {
            Id = 3,
            CustomerName = "Sasuke Uchiha",
            CustomerNIC = "456789123V",
            CheckIn = DateTime.Today.AddDays(7),
            CheckOut = DateTime.Today.AddDays(10),
            NumberOfGuests = 1,
            RoomTypes = [RoomType.SINGLE],
            SpecialRequest = "Extra pillows",
            IsRecurring = false
        });
    }

    public List<Booking> GetAll() => _bookings;
    public Booking? GetById(int id) => _bookings.FirstOrDefault(b => b.Id == id);

    public int Add(Booking booking)
    {
        // Check if all requested room types are available
        var availableRooms = _roomService.GetAll()
            .Where(r => booking.RoomTypes.Contains(r.Type) && r.IsAvailable)
            .ToList();

        if (availableRooms.Count == booking.RoomTypes.Count)
        {
            booking.Id = _bookings.Count + 1;
            _bookings.Add(booking);

            // Reserve each room
            foreach (var room in availableRooms)
            {
                room.IsAvailable = false;
                _roomService.Update(room);
            }

            if (!string.IsNullOrWhiteSpace(booking.SpecialRequest))
            {
                _requestService.Add(new SpecialRequest
                {
                    Id = _requestService.GetAll().Count + 1,
                    BookingId = booking.Id,
                    Request = booking.SpecialRequest,
                    Date = booking.CheckIn
                });
            }

            return booking.Id;
        }
        else
        {
            return 0;
        }
    }

    public void Update(Booking booking)
    {
        // Remove the old booking
        Delete(booking.Id);

        // Add the updated booking
        _bookings.Add(booking);

        // Update special requests
        var existingRequests = _requestService.GetByBookingId(booking.Id);
        foreach (var request in existingRequests)
        {
            _requestService.Delete(request.Id);
        }

        if (!string.IsNullOrWhiteSpace(booking.SpecialRequest))
        {
            _requestService.Add(new SpecialRequest
            {
                Id = _requestService.GetAll().Count + 1,
                BookingId = booking.Id,
                Request = booking.SpecialRequest,
                Date = booking.CheckIn
            });
        }
    }

    public void Delete(int id)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == id);
        if (booking == null) return;

        _bookings.RemoveAll(b => b.Id == id);
        var requests = _requestService.GetByBookingId(id);

        foreach (var request in requests)
        {
            _requestService.Delete(request.Id);
        }

        // Release all rooms associated with this booking
        var rooms = _roomService.GetAll()
            .Where(r => booking.RoomTypes.Contains(r.Type))
            .ToList();

        foreach (var room in rooms)
        {
            room.IsAvailable = true;
            _roomService.Update(room);
        }
    }
}