using Avalonia;
using System;
using Serilog;
using HealthCenter.Desktop.Infrastructure;

namespace HealthCenter.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        LoggingService.ConfigureLogging();
        
        try
        {
            SettingsService.Initialize();
            GlobalExceptionHandler.RegisterHandlers();
            
            Log.Information("Starting Health Center application");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            Log.Information("Application shut down gracefully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            LoggingService.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
