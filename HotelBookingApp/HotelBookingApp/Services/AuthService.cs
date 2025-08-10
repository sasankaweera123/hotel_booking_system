using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;

namespace HotelBookingApp.Services;

public class AuthService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
{
    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiClient");

            var payload = JsonSerializer.Serialize(new { username, password });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/auth/login", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var token = json.RootElement.GetProperty("token").GetString();

            if (string.IsNullOrEmpty(token))
                return false;

            contextAccessor.HttpContext!.Session.SetString("jwtToken", token);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await contextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return true;
        }catch (Exception ex)
        {
            // Log the exception (not implemented here)
            Console.WriteLine($"Login failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string username, string password, string role)
    {
        var client = httpClientFactory.CreateClient("ApiClient");

        var payload = JsonSerializer.Serialize(new { username, password, role });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/auth/register", content);

        return response.IsSuccessStatusCode;
    }

    public string? GetToken()
    {
        return contextAccessor.HttpContext!.Session.GetString("jwtToken");
    }

    public void Logout()
    {
        contextAccessor.HttpContext!.Session.Remove("jwtToken");
    }
}