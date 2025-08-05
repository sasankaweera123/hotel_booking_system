using System.Net.Http.Headers;
using System.Text.Json;
using HotelBookingApp.Dto;

namespace HotelBookingApp.Services;

public class ReportService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public ReportService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// Creates an HttpClient with JWT token from Session
    /// </summary>
    private HttpClient CreateClientWithAuth()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var token = _contextAccessor.HttpContext!.Session.GetString("jwtToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    /// <summary>
    /// Get full booking summary (no date filter)
    /// </summary>
    public async Task<List<ReportDto>> GetBookingSummaryAsync()
    {
        var client = CreateClientWithAuth();
        var response = await client.GetAsync("/reporting/summary");

        if (!response.IsSuccessStatusCode)
            return new List<ReportDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ReportDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    /// <summary>
    /// Get booking summary filtered by date range
    /// </summary>
    public async Task<List<ReportDto>> GetBookingSummaryAsync(DateTime fromDate, DateTime toDate)
    {
        var client = CreateClientWithAuth();

        // Build query string for filtering
        string url = $"/reporting/summary?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new List<ReportDto>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ReportDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}