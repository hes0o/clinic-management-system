# 🗄️ Bassam - Database Engineer
## بسام - مهندس قواعد البيانات

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** This sprint your job is to support the bug fixes by reviewing the database entities and ensuring the `TicketStatus` enum values have a proper Arabic display representation, so the UI can show human-readable status text instead of raw enum names like `Waiting`, `Called`, etc.

**Arabic:** مهمتك في هذا السبرينت هي دعم إصلاح الأخطاء من خلال مراجعة كيانات قاعدة البيانات والتأكد من أن قيم `TicketStatus` لها تمثيل عربي مقروء، حتى لا تظهر في الواجهة بأسمائها البرمجية.

---

## 🐛 Bug We Are Fixing

**Bug 1 — Raw Status Values in UI:**  
In the Nurse panel and Doctor panel, the ticket status shows as its raw enum name (e.g., `Waiting`, `AwaitingRecall`, `Called`) instead of a readable Arabic label. This must be fixed at the data/entity level.

---

## ✅ Task 1: Add Arabic Display For `TicketStatus` Enum

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours  
**File:** `/Database/Entities/QueueTicket.cs`

### English Instructions:
1. Open the file that defines the `TicketStatus` enum (likely inside `QueueTicket.cs` or a shared enums file)
2. Add a static extension method or a `Description` attribute to map each enum value to an Arabic label
3. The mapping should be:
   - `Waiting` → `"في الانتظار"`
   - `Called` → `"تم النداء"`
   - `InProgress` → `"قيد الفحص"`
   - `AwaitingRecall` → `"بانتظار إعادة النداء"`
   - `Completed` → `"منتهي"`
   - `Present` → `"حاضر"`

### Code Example:
```csharp
public static class TicketStatusExtensions
{
    public static string ToArabic(this TicketStatus status) => status switch
    {
        TicketStatus.Waiting        => "في الانتظار",
        TicketStatus.Called         => "تم النداء",
        TicketStatus.InProgress     => "قيد الفحص",
        TicketStatus.AwaitingRecall => "بانتظار إعادة النداء",
        TicketStatus.Completed      => "منتهي",
        TicketStatus.Present        => "حاضر",
        _                           => status.ToString()
    };
}
```

Place this class in the same file as `TicketStatus`, or in a new file `/Database/Entities/EnumExtensions.cs`.

> **Note:** The UI team (Wissam, Ela) will then use `{Binding Status}` differently — coordinate with them to use `ToArabic()` in a value converter or by exposing a computed property.

---

## ✅ Task 2: Verify No Raw Exception Leaks from DB Layer

**Priority:** 🟡 Medium | **Estimated Time:** 1 hour  
**File:** `/Database/HealthCenterDbContext.cs`

### English Instructions:
1. Review any direct DB calls that might throw raw exceptions to the UI
2. Ensure `EnsureCreated()` failures are caught and handled gracefully
3. If there is any `try/catch` missing around migrations or seeding, add it
4. Any caught exception should be logged (if logging is set up by Ahmed) — do NOT rethrow raw exceptions to the UI

### Code Example:
```csharp
try
{
    _db.Database.EnsureCreated();
}
catch (Exception ex)
{
    // Log it (use Ahmed's LoggingService when available)
    Console.Error.WriteLine($"DB init failed: {ex.Message}");
    // Show user-friendly message via ShowError()
}
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/database` |
| **Work Directory** | `/Database` folder ONLY |
| **Merge To** | `develop` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout feature/database
git pull origin develop
# ... do your work ...
git add .
git commit -m "fix(db): add Arabic display for TicketStatus enum"
git push origin feature/database
```

---

## ⚠️ Important Notes

- Do NOT edit files outside `/Database`
- Always test with `dotnet build` before committing
- Coordinate with Wissam and Ela on how they will consume the `ToArabic()` extension

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
git push origin feature/database
```

---

**Questions?** Ask Hassan (Team Lead)
