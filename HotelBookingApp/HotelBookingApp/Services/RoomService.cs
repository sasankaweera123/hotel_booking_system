using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HotelBookingApp.Dto;

namespace HotelBookingApp.Services;

public class RoomService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public RoomService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _contextAccessor = contextAccessor;
    }

    private HttpClient CreateClientWithAuth()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var token = _contextAccessor.HttpContext!.Session.GetString("jwtToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<List<RoomDto>> GetRoomsAsync()
    {
        var client = CreateClientWithAuth();
        var response = await client.GetAsync("/hotel/room");

        if (!response.IsSuccessStatusCode)
            return new List<RoomDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<RoomDto>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        var client = CreateClientWithAuth();
        var response = await client.GetAsync($"/hotel/room/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RoomDto>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> CreateRoomAsync(RoomDto room)
    {
        var client = CreateClientWithAuth();
        var content = new StringContent(JsonSerializer.Serialize(room), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/hotel/room", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateRoomAsync(int id, RoomDto room)
    {
        Console.WriteLine($"Updating room with ID {id}");
        var client = CreateClientWithAuth();
        var content = new StringContent(JsonSerializer.Serialize(room), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/hotel/room/{id}", content);
    
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Successfully updated room with ID {id}");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to update room with ID {id}. Status: {response.StatusCode}, Error: {error}");
            return false;
        }
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var client = CreateClientWithAuth();
        var response = await client.DeleteAsync($"/hotel/room/{id}");
        return response.IsSuccessStatusCode;
    }
    
    public async Task<List<HotelDto>> GetHotelsAsync()
    {
        var client = CreateClientWithAuth();
        var response = await client.GetAsync("/hotel");

        if (!response.IsSuccessStatusCode)
            return new List<HotelDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<HotelDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<List<RoomTypeDto>> GetRoomTypesAsync()
    {
        var client = CreateClientWithAuth();
        var response = await client.GetAsync("/hotel/roomtype");

        if (!response.IsSuccessStatusCode)
            return new List<RoomTypeDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<RoomTypeDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    
    public async Task<List<RoomDto>> GetAvailableRoomsByHotelAsync(int hotelId)
    {
        var client = CreateClientWithAuth();
        var rooms = await client.GetFromJsonAsync<List<RoomDto>>($"/hotel/room/by-hotel/{hotelId}");
        return rooms?.Where(r => r.IsAvailable).ToList() ?? new List<RoomDto>();
    }

}