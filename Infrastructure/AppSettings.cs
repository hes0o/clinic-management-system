namespace HealthCenter.Desktop.Infrastructure;

public class AppSettings
{
    public string ClinicName { get; set; } = string.Empty;
    public DatabaseSettings Database { get; set; } = new();
    public WorkingHoursSettings WorkingHours { get; set; } = new();
    public string Language { get; set; } = "ar";
}

public class DatabaseSettings
{
    public string Path { get; set; } = string.Empty;
}

public class WorkingHoursSettings
{
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
}
