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
            CustomerNic = "123456789V",
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(2),
            NumberOfGuests = 2,
            RoomTypes = [new BookingRoomType { Id = 1, RoomType = RoomType.SUITE }],
            SpecialRequest = "Late check-in",
            IsRecurring = false
        });
        
        _bookings.Add(new Booking
        {
            Id = 2,
            CustomerName = "Sakura Haruno",
            CustomerNic = "987654321V",
            CheckIn = DateTime.Today.AddDays(3),
            CheckOut = DateTime.Today.AddDays(5),
            NumberOfGuests = 10,
            RoomTypes = [
                new BookingRoomType { Id = 2, RoomType = RoomType.SUITE },
                new BookingRoomType { Id = 3, RoomType = RoomType.DOUBLE }
            ],
            SpecialRequest = "Ocean view",
            IsRecurring = true
        });
        
        _bookings.Add(new Booking
        {
            Id = 3,
            CustomerName = "Sasuke Uchiha",
            CustomerNic = "456789123V",
            CheckIn = DateTime.Today.AddDays(7),
            CheckOut = DateTime.Today.AddDays(10),
            NumberOfGuests = 1,
            RoomTypes = [
                new BookingRoomType { Id = 4, RoomType = RoomType.SINGLE },
            ],
            SpecialRequest = "Extra pillows",
            IsRecurring = false
        });
    }

    public List<Booking> GetAll() => _bookings;
    public Booking? GetById(int id) => _bookings.FirstOrDefault(b => b.Id == id);

    public int Add(Booking booking)
    {
        var requiredRoomGroups = booking.RoomTypes
            .GroupBy(rt => rt.RoomType)
            .ToDictionary(g => g.Key, g => g.Count());

        var roomsToReserve = new List<Room>();

        foreach (var roomType in requiredRoomGroups.Keys)
        {
            var availableRooms = _roomService.GetAvailableRooms(roomType)
                                             .Where(r => r.IsAvailable)
                                             .Take(requiredRoomGroups[roomType])
                                             .ToList();

            if (availableRooms.Count < requiredRoomGroups[roomType])
            {
                return 0;
            }

            roomsToReserve.AddRange(availableRooms);

            foreach (var room in availableRooms)
            {
                // Update the corresponding BookingRoomType to use the reserved room ID
                var bookingRoom = booking.RoomTypes
                    .FirstOrDefault(br => br.RoomType == room.Type && br.Id == 0); // Match unassigned ones
                if (bookingRoom != null)
                {
                    bookingRoom.Id = room.Id;
                }
            }

        }
        
        booking.Id = _bookings.Count + 1;
        _bookings.Add(booking);

        foreach (var room in roomsToReserve)
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

    public void Update(Booking booking)
    {
        // Remove the old booking
        Delete(booking.Id);
        
        if (booking.CheckIn >= booking.CheckOut || booking.RoomTypes.Count == 0)
        {
            return;
        }

        // Add the updated booking
        Add(booking);

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
        
        var requests = _requestService.GetByBookingId(id);

        foreach (var request in requests)
        {
            _requestService.Delete(request.Id);
        }
        
        var roomIds = booking.RoomTypes.Select(rt => rt.Id).ToList();
        
        var rooms = _roomService.GetAll().Where(r => roomIds.Contains(r.Id)).ToList();
        Console.WriteLine($"Rooms found: {string.Join(", ", rooms.Select(r => $"Id={r.Id}, Type={r.Type}, Available={r.IsAvailable}"))}");
        

        foreach (var room in rooms)
        {
            _roomService.UpdateAvailability(room.Id, true);
        }
        
        _bookings.RemoveAll(b => b.Id == id);
    }
}