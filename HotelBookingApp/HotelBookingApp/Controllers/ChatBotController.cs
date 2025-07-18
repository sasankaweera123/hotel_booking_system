using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;
namespace HotelBookingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatBotController(ChatBotService chatBotService) : ControllerBase
{
    [HttpPost("message")]
    public IActionResult PostMessage([FromBody] ChatMessage message)
    {
        try
        {
            var response = chatBotService.GetResponse(message);
            return Ok(new ChatMessage { Text = response });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ChatMessage { Text = "An internal error occurred: " + ex.Message });
        }
    }
}
