using System.IO;
using Serilog;
using Serilog.Events;

namespace HealthCenter.Desktop.Infrastructure;

public static class LoggingService
{
    public static void ConfigureLogging()
    {
        Directory.CreateDirectory("Logs");
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Avalonia", LogEventLevel.Information)
            .WriteTo.File("Logs/healthcenter-.log", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 31)
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}
