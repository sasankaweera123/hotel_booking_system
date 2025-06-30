using HotelBookingApp.Models;

namespace HotelBookingApp.Services;

public class RequestService
{
    private readonly List<SpecialRequest> _requests = new();

    public RequestService()
    {
        _requests.Add(new SpecialRequest { Id = 1, Request = "Extra pillow", Date = DateTime.Today });
    }

    public List<SpecialRequest> GetAll() => _requests;
    public SpecialRequest? GetById(int id) => _requests.FirstOrDefault(r => r.Id == id);
    public void Add(SpecialRequest request) => _requests.Add(request);
    public void Update(SpecialRequest request) { Delete(request.Id); _requests.Add(request); }
    public void Delete(int id) => _requests.RemoveAll(r => r.Id == id);
}