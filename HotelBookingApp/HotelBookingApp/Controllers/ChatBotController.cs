using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotController(ChatBotService chatBotService, LogQueue logQueue) : ControllerBase
    {
        [HttpPost("message")]
        public async Task<IActionResult> PostMessage([FromBody] ChatMessage message)
        {
            try
            {
                logQueue.Queue.Enqueue(new LogMessage
                {
                    Level = "INFO",
                    Message = "Received message from user: " + message.Text
                });

                if (message == null || string.IsNullOrWhiteSpace(message.Text))
                    return BadRequest(new { reply = "Message cannot be empty." });

                // Call updated async chatbot logic
                var response = await chatBotService.GetResponseAsync(message);

                // Return JSON reply for frontend
                return Ok(new { text = response });
            }
            catch (Exception ex)
            {
                logQueue.Queue.Enqueue(new LogMessage
                {
                    Level = "ERROR",
                    Message = "Error processing message: " + ex.Message
                });
                return StatusCode(500, new { reply = "An internal error occurred: " + ex.Message });
            }
        }
    }
}