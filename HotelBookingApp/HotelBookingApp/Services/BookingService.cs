using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HotelBookingApp.Dto;

namespace HotelBookingApp.Services;

public class BookingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public BookingService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
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

    public async Task<List<BookingDto>> GetMyBookingsAsync()
    {
        var client = CreateClientWithAuth();
        var response = await client.GetAsync("/booking/mybookings");

        if (!response.IsSuccessStatusCode)
            return new List<BookingDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<BookingDto>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var client = CreateClientWithAuth();
        var response = await client.GetAsync($"/booking/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BookingDto>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> CreateBookingAsync(BookingDto booking)
    {
        var client = CreateClientWithAuth();
        var content = new StringContent(JsonSerializer.Serialize(booking), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/booking", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Booking created successfully.");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to create booking. Status: {response.StatusCode}, Error: {error}");
            return false;
        }
    }

    public async Task<bool> UpdateBookingAsync(int id, BookingDto booking)
    {
        Console.WriteLine($"Updating booking with ID {id}");
        var client = CreateClientWithAuth();
        var content = new StringContent(JsonSerializer.Serialize(booking), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/booking/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Successfully updated booking with ID {id}");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to update booking with ID {id}. Status: {response.StatusCode}, Error: {error}");
            return false;
        }
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        var client = CreateClientWithAuth();
        var response = await client.DeleteAsync($"/booking/{id}");
        return response.IsSuccessStatusCode;
    }
}