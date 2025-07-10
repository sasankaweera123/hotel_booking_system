using System.Text.Json;
using System.Text.RegularExpressions;
using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Services;

public class ChatBotService(ReportService reportService, RoomService roomService, BookingService bookingService)
{
    public string GetResponse(ChatMessage message)
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chatbot-intents.json");

        if (!File.Exists(jsonPath))
            return $"Intent file not found at: {jsonPath}";

        var jsonContent = File.ReadAllText(jsonPath);
        if (string.IsNullOrWhiteSpace(jsonContent))
            return "Intent file is empty.";

        var intents = JsonSerializer.Deserialize<List<ChatIntent>>(jsonContent);
        if (intents == null)
            return "Could not parse intents JSON.";

        var userInput = message.Text.Trim();

        foreach (var intent in intents)
        {
            if (intent.Patterns == null) continue;

            foreach (var pattern in intent.Patterns.Where(pattern => Regex.IsMatch(userInput, Regex.Escape(pattern), RegexOptions.IgnoreCase)))
            {
                switch (intent.Intent)
                {
                    case "predict_availability":
                        var reports1 = reportService.GenerateReports(DateTime.Today, DateTime.Today.AddDays(7));
                        var avgRooms = reports1.Average(r => r.TotalRoomsBooked);
                        return $"On average, {avgRooms} rooms are booked daily next week. Availability is {(avgRooms < 5 ? "high" : "limited")}.";

                    case "predict_pricing":
                        var reports2 = reportService.GenerateReports(DateTime.Today, DateTime.Today.AddDays(30));
                        var avgRevenue = reports2.Average(r => r.TotalRevenue);
                        return $"Average daily revenue for the next month is {avgRevenue:C}. Prices may increase during peak days.";

                    case "check_room_by_date":
                        var dateMatch = Regex.Match(userInput, @"(?:on|from)\s+(\w+\s+\d{1,2})", RegexOptions.IgnoreCase);
                        if (!dateMatch.Success || !DateTime.TryParse(dateMatch.Groups[1].Value, out var parsedDate))
                            return "Please provide a valid date (e.g., 'on July 20').";
                        var availableRooms = roomService.GetAll()
                            .Where(r => r.IsAvailable)
                            .ToList();

                        return availableRooms.Count > 0
                            ? $"Yes, we have {availableRooms.Count} available rooms on {parsedDate:MMMM dd}."
                            : $"Sorry, no rooms are available on {parsedDate:MMMM dd}.";

                    case "booking_lookup":
                        var nicMatch = Regex.Match(userInput, @"\b\d{9}[VXvx]\b");
                        if (!nicMatch.Success) return "Please provide a valid NIC number (e.g., 987654321V).";
                        var nic = nicMatch.Value.ToUpper();
                        var bookings = bookingService.GetAll()
                            .Where(b => b.CustomerNic?.Equals(nic, StringComparison.OrdinalIgnoreCase) == true)
                            .ToList();

                        if (bookings.Count == 0)
                            return $"No bookings found for NIC: {nic}.";

                        return string.Join("\n\n", bookings.Select(b =>
                            $"📘 Booking #{b.Id}\n🛏 Room(s): {string.Join(", ", b.RoomTypes.Select(r => r.RoomType))}\n📅 Check-In: {b.CheckIn:MMM dd}\n🏁 Check-Out: {b.CheckOut:MMM dd}"));

                    default:
                        return intent.Response ?? "Processing your request...";
                }
            }
        }

        // Handle fallback intent explicitly if defined
        var fallback = intents.FirstOrDefault(i => i.Intent == "fallback");
        return fallback?.Response ?? "Sorry, I didn't understand that. Can you rephrase?";
    }
}
