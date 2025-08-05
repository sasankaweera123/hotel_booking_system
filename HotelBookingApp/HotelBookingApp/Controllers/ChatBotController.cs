using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotController : ControllerBase
    {
        private readonly ChatBotService _chatBotService;

        public ChatBotController(ChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }
        
        [HttpPost("message")]
        public async Task<IActionResult> PostMessage([FromBody] ChatMessage message)
        {
            try
            {
                if (message == null || string.IsNullOrWhiteSpace(message.Text))
                    return BadRequest(new { reply = "Message cannot be empty." });

                // Call updated async chatbot logic
                var response = await _chatBotService.GetResponseAsync(message);

                // Return JSON reply for frontend
                return Ok(new { text = response }); 

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { reply = "An internal error occurred: " + ex.Message });
            }
        }
    }
}