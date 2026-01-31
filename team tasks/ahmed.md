# ⚙️ Ahmed - Infrastructure Engineer
## أحمد - مهندس البنية التحتية

---

## 📋 Role Overview

**English:** Responsible for logging, error handling, configuration, and application settings.

**Arabic:** مسؤول عن السجلات ومعالجة الأخطاء والإعدادات.

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Branch** | `feature/infrastructure` |
| **Directory** | `/Infrastructure` folder |
| **Merge To** | `develop` (via PR) |

```bash
git checkout -b feature/infrastructure
git push -u origin feature/infrastructure
```

---

## ✅ Task 1: Setup Serilog Logging

**Priority:** 🔴 High | **Time:** 2 hours
**File:** Create `/Infrastructure/LoggingService.cs`

### Instructions:
1. Configure Serilog in Program.cs
2. Log to file: `/Logs/healthcenter-{date}.log`
3. Log levels: Debug, Info, Warning, Error
4. Include timestamp and context

### Code Example:
```csharp
// In Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs/healthcenter-.log", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```

---

## ✅ Task 2: Global Error Handling

**Priority:** 🔴 High | **Time:** 1.5 hours

### Instructions:
1. Create global exception handler
2. Log all unhandled exceptions
3. Show user-friendly Arabic error message
4. Don't crash the app on errors

### Code Example:
```csharp
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    Log.Fatal(e.ExceptionObject as Exception, "خطأ غير متوقع");
    // Show dialog to user
};
```

---

## ✅ Task 3: Application Settings

**Priority:** 🟡 Medium | **Time:** 1.5 hours
**File:** Create `appsettings.json`

### Instructions:
1. Create settings file with:
   - Database path
   - Clinic name
   - Working hours
   - Default language
2. Load settings at startup

### Example:
```json
{
  "ClinicName": "المركز الصحي",
  "Database": {
    "Path": "healthcenter.db"
  },
  "WorkingHours": {
    "Start": "08:00",
    "End": "22:00"
  }
}
```

---

## 📁 Your Files

```
/Infrastructure/
├── LoggingService.cs     ← Create
├── ErrorHandler.cs       ← Create
└── SettingsService.cs    ← Create

appsettings.json          ← Create in root
```

**Questions?** Ask Hassan
