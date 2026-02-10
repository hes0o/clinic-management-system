using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Serilog;

namespace HealthCenter.Desktop.Features.Queue.Services;

public class AudioNotificationService
{
    public async Task PlayNotificationAsync(int ticketNumber)
    {
        try
        {
            await PlayChimeAsync();
            await Task.Delay(500);
            await AnnounceTicketNumberAsync(ticketNumber);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error playing audio notification");
        }
    }

    private async Task PlayChimeAsync()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            try
            {
                Process.Start("afplay", "/System/Library/Sounds/Glass.aiff");
            }
            catch
            {
                // Fallback or ignore
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                Console.Beep(800, 500);
            }
            catch { }
        }
        await Task.CompletedTask;
    }

    private async Task AnnounceTicketNumberAsync(int number)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            try
            {
                var text = $"رقم {number}";
                Process.Start("say", $"-v Tarik \"{text}\"");
            }
            catch { }
        }
        await Task.CompletedTask;
    }
}

