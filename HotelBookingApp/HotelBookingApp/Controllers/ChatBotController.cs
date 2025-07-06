using System.Text.Json;
using System.Text.RegularExpressions;
using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;
namespace HotelBookingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatBotController : ControllerBase
{
    private readonly ReportService _reportService = new();

    [HttpPost("message")]
    public IActionResult PostMessage([FromBody] ChatMessage message)
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chatbot-intents.json");

            if (!System.IO.File.Exists(jsonPath))
            {
                return BadRequest(new ChatMessage { Text = $"Intent file not found at: {jsonPath}" });
            }

            var jsonContent = System.IO.File.ReadAllText(jsonPath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return BadRequest(new ChatMessage { Text = "Intent file is empty." });
            }

            var intents = JsonSerializer.Deserialize<List<ChatIntent>>(jsonContent);
            if (intents == null)
            {
                return BadRequest(new ChatMessage { Text = "Could not parse intents JSON." });
            }

            foreach (var intent in intents)
            {
                if (intent.Patterns == null) continue;

                foreach (var pattern in intent.Patterns)
                {
                    if (Regex.IsMatch(message.Text, Regex.Escape(pattern), RegexOptions.IgnoreCase))
                    {
                        if (intent.Intent == "predict_availability")
                        {
                            var reports = _reportService.GenerateReports(DateTime.Now, DateTime.Now.AddDays(7));
                            var avgRooms = reports.Average(r => r.TotalRoomsBooked);
                            var response =
                                $"On average, {avgRooms} rooms are booked daily next week. Availability is {(avgRooms < 5 ? "high" : "limited")}.";
                            return Ok(new ChatMessage { Text = response });
                        }

                        if (intent.Intent == "predict_pricing")
                        {
                            var reports = _reportService.GenerateReports(DateTime.Now, DateTime.Now.AddDays(30));
                            var avgRevenue = reports.Average(r => r.TotalRevenue);
                            var response =
                                $"Average daily revenue for the next month is {avgRevenue:C}. Prices may increase during peak days.";
                            return Ok(new ChatMessage { Text = response });
                        }

                        return Ok(new ChatMessage { Text = intent.Response });
                    }
                }
            }

            return Ok(new ChatMessage { Text = "Sorry, I didn't understand that. Can you rephrase?" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ChatMessage { Text = "An internal error occurred: " + ex.Message });
        }
    }
}