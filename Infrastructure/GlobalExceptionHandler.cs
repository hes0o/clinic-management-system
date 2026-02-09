using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCenter.Desktop.Infrastructure;

public static class GlobalExceptionHandler
{
    private static int _isHandlingException = 0;

    public static void RegisterHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        
        TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;

        Log.Information("Global exception handlers registered");
    }

    public static void RegisterAvaloniaHandlers()
    {
        try
        {
            Dispatcher.UIThread.UnhandledException += (s, e) =>
            {
                HandleAvaloniaException(e.Exception);
                e.Handled = true;
            };
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to register Dispatcher.UIThread.UnhandledException handler");
        }

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (s, e) =>
            {
                Log.Information("Application exiting with code: {ExitCode}", e.ApplicationExitCode);
                LoggingService.CloseAndFlush();
            };
        }

        Log.Information("Avalonia-specific exception handlers registered");
    }

    private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            var errorId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            
            Log.Fatal(exception, "خطأ غير متوقع في نطاق التطبيق. رقم الخطأ: {ErrorId}", errorId);

            if (Interlocked.CompareExchange(ref _isHandlingException, 1, 0) == 0)
            {
                try
                {
                    ShowErrorToUser(exception, errorId, e.IsTerminating);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "فشل في عرض رسالة الخطأ للمستخدم");
                }
                finally
                {
                    Interlocked.Exchange(ref _isHandlingException, 0);
                }
            }

            if (e.IsTerminating)
            {
                Log.Fatal("التطبيق سيتم إغلاقه بسبب خطأ حرج");
                LoggingService.CloseAndFlush();
            }
            else
            {
                Log.CloseAndFlush();
            }
        }
    }

    private static void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var errorId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        
        Log.Error(e.Exception, "استثناء غير ملحوظ في مهمة غير متزامنة. رقم الخطأ: {ErrorId}", errorId);

        e.SetObserved();

        if (Interlocked.CompareExchange(ref _isHandlingException, 1, 0) == 0)
        {
            try
            {
                ShowErrorToUser(e.Exception, errorId, false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "فشل في عرض رسالة الخطأ للمستخدم");
            }
            finally
            {
                Interlocked.Exchange(ref _isHandlingException, 0);
            }
        }
    }

    public static void HandleAvaloniaException(Exception exception)
    {
        var errorId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        
        Log.Error(exception, "استثناء في واجهة المستخدم Avalonia. رقم الخطأ: {ErrorId}", errorId);

        if (Interlocked.CompareExchange(ref _isHandlingException, 1, 0) == 0)
        {
            try
            {
                ShowErrorToUser(exception, errorId, false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "فشل في عرض رسالة الخطأ للمستخدم");
            }
            finally
            {
                Interlocked.Exchange(ref _isHandlingException, 0);
            }
        }
    }

    private static void ShowErrorToUser(Exception exception, string errorId, bool isTerminating)
    {
        if (isTerminating)
        {
            return;
        }

        if (Application.Current?.ApplicationLifetime == null)
        {
            Log.Warning("UI not available yet; skipping error dialog");
            return;
        }

        try
        {
            if (!Dispatcher.UIThread.CheckAccess())
            {
                try
                {
                    Dispatcher.UIThread.Post(async () => 
                    {
                        await ErrorDialogService.ShowFatalErrorDialogAsync(exception, errorId);
                    }, DispatcherPriority.Send);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to post error dialog to UI thread");
                }
            }
            else
            {
                _ = ErrorDialogService.ShowFatalErrorDialogAsync(exception, errorId);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Emergency fallback: Failed to show error dialog");
        }
    }
}
