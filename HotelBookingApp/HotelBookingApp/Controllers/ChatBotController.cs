using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;
namespace HotelBookingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatBotController : ControllerBase
{
    private readonly ChatBotService _chatBotService;

    public ChatBotController()
    {
        var reportService = new ReportService();
        var roomService = new RoomService();
        var bookingService = new BookingService();
        _chatBotService = new ChatBotService(reportService, roomService, bookingService);
    }

    [HttpPost("message")]
    public IActionResult PostMessage([FromBody] ChatMessage message)
    {
        try
        {
            var response = _chatBotService.GetResponse(message);
            return Ok(new ChatMessage { Text = response });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ChatMessage { Text = "An internal error occurred: " + ex.Message });
        }
    }
}