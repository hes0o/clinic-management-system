using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Threading;
using Serilog;
using System;
using System.Threading.Tasks;

namespace HealthCenter.Desktop.Infrastructure;

public static class ErrorDialogService
{
    public static async Task ShowErrorDialogAsync(string message, string title = "خطأ في التطبيق", string details = "")
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            await ShowDialogInternal(message, title, details);
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(async () => await ShowDialogInternal(message, title, details));
        }
    }

    public static void ShowErrorDialog(string message, string title = "خطأ في التطبيق", string details = "")
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            _ = ShowDialogInternal(message, title, details);
        }
        else
        {
            Dispatcher.UIThread.InvokeAsync(async () => await ShowDialogInternal(message, title, details));
        }
    }

    public static async Task ShowFatalErrorDialogAsync(Exception exception, string errorId)
    {
        var message = $"حدث خطأ غير متوقع. تم حفظ تفاصيل الخطأ في ملف السجل.\n\nرقم الخطأ: {errorId}\nالوقت: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        var title = "خطأ في التطبيق";
        
        await ShowErrorDialogAsync(message, title, "");
    }

    private static async Task ShowDialogInternal(string message, string title, string details)
    {
        try
        {
            var window = GetMainWindow();
            if (window == null)
            {
                Log.Warning("Main window not available, creating standalone error dialog");
                await ShowStandaloneDialog(message, title);
                return;
            }

            var messageWithDetails = string.IsNullOrEmpty(details) 
                ? message 
                : $"{message}\n\n{details}";

            var messageBox = new Window
            {
                Title = title,
                Width = 500,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = messageWithDetails,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            MaxHeight = 150
                        },
                        new Button
                        {
                            Content = "موافق",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Width = 100
                        }
                    }
                }
            };

            var button = (messageBox.Content as StackPanel)?.Children[1] as Button;
            if (button != null)
            {
                button.Click += (s, e) => messageBox.Close();
            }

            await messageBox.ShowDialog(window);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to show error dialog");
            try
            {
                await ShowStandaloneDialog(message, title);
            }
            catch (Exception fallbackEx)
            {
                Log.Fatal(fallbackEx, "Fallback error dialog also failed");
            }
        }
    }

    private static Task ShowStandaloneDialog(string message, string title)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 500,
            Height = 250,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 15,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        MaxHeight = 150
                    },
                    new Button
                    {
                        Content = "موافق",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 100
                    }
                }
            }
        };

        var button = (messageBox.Content as StackPanel)?.Children[1] as Button;
        if (button != null)
        {
            button.Click += (s, e) => messageBox.Close();
        }

        messageBox.Show();
        return Task.CompletedTask;
    }

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }
}
