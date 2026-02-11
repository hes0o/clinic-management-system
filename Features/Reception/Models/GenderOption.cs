namespace HealthCenter.Desktop.Features.Reception.Models;

public class GenderOption
{
    public string Display { get; set; }
    public string Value { get; set; }

    public GenderOption(string display, string value)
    {
        Display = display;
        Value = value;
    }
    public override string ToString() => Display;
}