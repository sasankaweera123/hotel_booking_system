using HotelBookingApp.Models;

namespace HotelBookingApp.Services;

public class RequestService
{
    private readonly List<SpecialRequest> _requests = new();

    public RequestService()
    {
        _requests.Add(new SpecialRequest 
        { 
            Id = 1,
            BookingId = 3,
            Request = "Extra pillow", 
            Date = DateTime.Today 
        });
        _requests.Add(new SpecialRequest 
        { 
            Id = 2,
            BookingId = 1,
            Request = "Late check-in", 
            Date = DateTime.Today 
        });
        _requests.Add(new SpecialRequest 
        { 
            Id = 3,
            BookingId = 2,
            Request = "Ocean view", 
            Date = DateTime.Today 
        });
    }

    public List<SpecialRequest> GetAll() => _requests;
    public SpecialRequest? GetById(int id) => _requests.FirstOrDefault(r => r.Id == id);
    public void Add(SpecialRequest request) => _requests.Add(request);
    public void Update(SpecialRequest request) { Delete(request.Id); _requests.Add(request); }
    public void Delete(int id) => _requests.RemoveAll(r => r.Id == id);
    public List<SpecialRequest> GetByBookingId(int bookingId) 
        => _requests.Where(r => r.BookingId == bookingId).ToList();
    
}