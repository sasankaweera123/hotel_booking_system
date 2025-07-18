using HotelBookingApp.Models;

namespace HotelBookingApp.Services;

public class ReportService(RequestService requestService, RoomService roomService, BookingService bookingService)
{
    public List<Report> GenerateReports(DateTime fromDate, DateTime toDate)
    {
        var reports = new List<Report>();

        var allBookings = bookingService.GetAll();
        var allRooms = roomService.GetAll();
        var allRequests = requestService.GetAll();

        
        if (fromDate > toDate)
            throw new ArgumentException("fromDate must be earlier than toDate");

        
        var filteredBookings = allBookings
            .Where(b => b.CheckIn.Date >= fromDate.Date && b.CheckIn.Date <= toDate.Date)
            .ToList();

        var filteredRequests = allRequests
            .Where(r => r.Date.Date >= fromDate.Date && r.Date.Date <= toDate.Date)
            .ToList();

        // Generate daily reports within the range
        for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
        {
            var bookingsForDay = filteredBookings
                .Where(b => b.CheckIn.Date == date)
                .ToList();

            var requestsForDay = filteredRequests
                .Where(r => r.Date.Date == date)
                .ToList();

            var totalRoomsBooked = bookingsForDay.Sum(b => b.RoomTypes?.Count ?? 0);
            var totalRevenue = bookingsForDay.Sum(b =>
            {
                return b.RoomTypes.Select(bookedRoomType => allRooms.FirstOrDefault(r => r.Type == bookedRoomType.RoomType)).Select(room => room?.Price ?? 0).Sum();
            });

            reports.Add(new Report
            {
                StartDate = date,
                EndDate = date,
                TotalBookings = bookingsForDay.Count,
                TotalRoomsBooked = totalRoomsBooked,
                TotalRevenue = totalRevenue,
                Comments = $"{requestsForDay.Count} special requests"
            });
        }

        return reports;
    }

}
