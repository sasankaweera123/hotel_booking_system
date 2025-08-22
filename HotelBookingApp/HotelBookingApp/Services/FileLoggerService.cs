using System.Text;

namespace HotelBookingApp.Services;

public class FileLoggerService : BackgroundService
{
    private readonly LogQueue _logQueue;
    private readonly string _logDirectory;
    private readonly string _logFile;

    public FileLoggerService(LogQueue logQueue)
    {
        _logQueue = logQueue;
        // var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        // _logDirectory = Path.Combine(baseDir, "ClientLogs");
        _logDirectory = Path.Combine(@"C:\", "ClientLogs");
        Directory.CreateDirectory(_logDirectory);

        _logFile = Path.Combine(_logDirectory, $"client-log-{DateTime.Now:yyyyMMdd}.txt");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logQueue.Queue.TryDequeue(out var log))
            {
                var logEntry = $"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] [{log.Level}] {log.Message}\n";

                // ThreadPool + async I/O
                await Task.Run(async () =>
                {
                    await File.AppendAllTextAsync(_logFile, logEntry, Encoding.UTF8, stoppingToken);
                }, stoppingToken);
            }
            else
            {
                // Sleep a bit to reduce CPU usage when idle
                await Task.Delay(200, stoppingToken);
            }
        }
    }
}