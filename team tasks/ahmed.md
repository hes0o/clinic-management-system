# ⚙️ Ahmed - Infrastructure Engineer
## أحمد - مهندس البنية التحتية

---

## 📋 Role Overview

**English:** This sprint your job is to create a global error handling layer so that raw C# exceptions never appear in the UI. You will also create a reusable value converter so the team can display friendly Arabic status labels without code changes all over the place.

**Arabic:** مهمتك في هذا السبرينت هي إنشاء طبقة للتعامل مع الأخطاء على مستوى التطبيق كله، حتى لا تظهر أخطاء C# الخام في الواجهة. ستنشئ أيضاً محول قيم (Value Converter) قابلاً لإعادة الاستخدام لعرض الحالات بالعربية.

---

## 🐛 Bug We Are Fixing

**Bug 1 — Raw Error Messages:**  
If any unhandled exception reaches the UI (e.g., EF Core failure, null reference, etc.), the user currently sees raw English error text or the app crashes. We need a global safety net that catches these and shows a friendly Arabic error banner.

---

## ✅ Task 1: Global Exception Handler

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours  
**File:** `/Infrastructure/GlobalExceptionHandler.cs` (new file)

### Instructions:
1. Create `/Infrastructure/GlobalExceptionHandler.cs`
2. Hook into `AppDomain.CurrentDomain.UnhandledException` and `TaskScheduler.UnobservedTaskException`
3. When an unhandled exception occurs:
   - Log it (console or file)
   - Show a friendly dialog or notification to the user in Arabic
   - Do NOT let the app crash silently

### Code Example:
```csharp
public static class GlobalExceptionHandler
{
    public static void Register()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            Console.Error.WriteLine($"[FATAL] {ex?.Message}");
            // Show a friendly Avalonia dialog
            ShowFriendlyError("حدث خطأ غير متوقع. يرجى إعادة تشغيل التطبيق.");
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            Console.Error.WriteLine($"[TASK ERROR] {args.Exception.Message}");
            args.SetObserved(); // prevent crash
        };
    }

    private static void ShowFriendlyError(string message)
    {
        // Use Avalonia dispatcher to show on UI thread
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            // Show a MessageBox or update a global status bar
            Console.WriteLine($"UI Error: {message}");
        });
    }
}
```

4. Call `GlobalExceptionHandler.Register()` at the top of `Program.cs` before the app starts.

---

## ✅ Task 2: Create `TicketStatusConverter` Value Converter

**Priority:** 🔴 High | **Estimated Time:** 1 hour  
**File:** `/Infrastructure/Converters/TicketStatusConverter.cs` (new file)

### Instructions:
1. Create the directory `/Infrastructure/Converters/` if it does not exist
2. Create `TicketStatusConverter.cs` implementing `IValueConverter`
3. It should convert a `TicketStatus` enum value to its Arabic string equivalent
4. Register it in `App.axaml` resources so all views can use it

### Code Example:
```csharp
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using HealthCenter.Desktop.Database.Entities;

public class TicketStatusConverter : IValueConverter
{
    public static readonly TicketStatusConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Waiting        => "في الانتظار",
                TicketStatus.Called         => "تم النداء",
                TicketStatus.InProgress     => "قيد الفحص",
                TicketStatus.AwaitingRecall => "بانتظار إعادة النداء",
                TicketStatus.Completed      => "منتهي",
                TicketStatus.Present        => "حاضر",
                _                           => value.ToString()
            };
        }
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
```

5. Register in `App.axaml`:
```xml
<Application.Resources>
    <converters:TicketStatusConverter x:Key="TicketStatusConverter"/>
</Application.Resources>
```

6. Document usage for the team — they use it in XAML like:
```xml
<TextBlock Text="{Binding Status, Converter={StaticResource TicketStatusConverter}}"/>
```

---

## 📁 Your Files

```
/Infrastructure/
├── GlobalExceptionHandler.cs     ← Create
└── Converters/
    └── TicketStatusConverter.cs  ← Create

Program.cs                        ← Edit (register handler)
App.axaml                         ← Edit (register converter)
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Branch** | `feature/infrastructure` |
| **Directory** | `/Infrastructure` + `App.axaml` + `Program.cs` |
| **Merge To** | `develop` (via PR) |

```bash
git checkout feature/infrastructure
git pull origin develop
git add .
git commit -m "feat(infra): global error handler + TicketStatus value converter"
git push origin feature/infrastructure
```

## 🧹 Code Formatting Rule (Mandatory)

> **A GitHub Action called "Clean Code Enforcer" will automatically reject your push if your code is not properly formatted.**

Before **every** `git push`, you MUST run:

```bash
dotnet format HealthCenter.Desktop.csproj
```

This auto-fixes all whitespace and formatting issues. If you skip this step your PR will **fail the CI check** and be blocked from merging.

```bash
# ✅ Full workflow before pushing:
dotnet format HealthCenter.Desktop.csproj
git add .
git commit -m "your message"
git push origin feature/infrastructure
```

---

**Questions?** Ask Hassan
