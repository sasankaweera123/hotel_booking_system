using System.Text.Json;
using System.Text.RegularExpressions;
using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Services
{
    public class ChatBotService(ReportService reportService, RoomService roomService, BookingService bookingService)
    {

        public async Task<string> GetResponseAsync(ChatMessage message)
        {
            // Load intents JSON
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chatbot-intents.json");

            if (!File.Exists(jsonPath))
                return $"Intent file not found at: {jsonPath}";

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            if (string.IsNullOrWhiteSpace(jsonContent))
                return "Intent file is empty.";

            var intents = JsonSerializer.Deserialize<List<ChatIntent>>(jsonContent);
            if (intents == null)
                return "Could not parse intents JSON.";

            var userInput = message.Text.Trim();

            foreach (var intent in intents)
            {
                if (intent.Patterns == null) continue;

                foreach (var pattern in intent.Patterns
                             .Where(p => Regex.IsMatch(userInput, Regex.Escape(p), RegexOptions.IgnoreCase)))
                {
                    switch (intent.Intent)
                    {
                        case "predict_availability":
                            var reports1 = await reportService.GetBookingSummaryAsync(DateTime.Today, DateTime.Today.AddDays(7));
                            var avgRooms = reports1.Average(r => r.TotalBookings);
                            return $"On average, {avgRooms} rooms are booked daily next week. Availability is {(avgRooms < 5 ? "high" : "limited")}.";

                        case "predict_pricing":
                            var reports2 = await reportService.GetBookingSummaryAsync(DateTime.Today, DateTime.Today.AddDays(30));
                            var avgRevenue = reports2.Average(r => r.TotalBookings * 100); // Adjust if you have revenue data
                            return $"Average daily booking volume next month is {avgRevenue}. Prices may increase on peak days.";

                        default:
                            return intent.Response ?? "Processing your request...";
                    }
                }
            }

            // Fallback
            var fallback = intents.FirstOrDefault(i => i.Intent == "fallback");
            return fallback?.Response ?? "Sorry, I didn't understand that. Can you rephrase?";
        }
    }
}
