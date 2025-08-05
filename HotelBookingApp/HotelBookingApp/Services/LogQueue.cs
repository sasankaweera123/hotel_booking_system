using System.Collections.Concurrent;
using HotelBookingApp.Models.Utils;

namespace HotelBookingApp.Services;

public class LogQueue
{
    public ConcurrentQueue<LogMessage> Queue { get; } = new();
}