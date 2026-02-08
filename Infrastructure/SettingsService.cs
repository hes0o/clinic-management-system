using System;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace HealthCenter.Desktop.Infrastructure;

public static class SettingsService
{
    private static AppSettings? _settings;
    private static readonly object _lock = new object();

    public static AppSettings Current
    {
        get
        {
            if (_settings == null)
            {
                throw new InvalidOperationException("Settings not initialized. Call Initialize() first.");
            }
            return _settings;
        }
    }

    public static void Initialize()
    {
        lock (_lock)
        {
            if (_settings != null)
            {
                return;
            }

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

                var settings = configuration.Get<AppSettings>() ?? new AppSettings();

                ValidateSettings(settings);

                _settings = settings;

                Log.Information("Settings loaded successfully. Clinic: {ClinicName}", _settings.ClinicName);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to load application settings");
                throw new InvalidOperationException("Failed to load application settings. Ensure appsettings.json exists and is valid.", ex);
            }
        }
    }

    private static void ValidateSettings(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ClinicName))
        {
            throw new InvalidOperationException("ClinicName is required in appsettings.json");
        }

        if (string.IsNullOrWhiteSpace(settings.Database.Path))
        {
            throw new InvalidOperationException("Database.Path is required in appsettings.json");
        }

        if (string.IsNullOrWhiteSpace(settings.WorkingHours.Start) || 
            string.IsNullOrWhiteSpace(settings.WorkingHours.End))
        {
            throw new InvalidOperationException("WorkingHours.Start and WorkingHours.End are required in appsettings.json");
        }
    }
}
