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

---

## 🆕 Task 3: Add Auto-Polling to Lab Panel & Remove Refresh Button

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours  
**Files:**
- `/Features/Lab/ViewModels/LabPanelViewModel.cs`
- `/Features/Lab/Views/LabPanelView.axaml`

### Instructions:
1. Open `LabPanelViewModel.cs`
2. Add `using Avalonia.Threading;` at the top
3. Add a `private readonly DispatcherTimer _refreshTimer;` field
4. In the constructor, initialize the timer to poll every 5 seconds:
```csharp
_refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
_refreshTimer.Tick += (s, e) => LoadTestsSilent();
_refreshTimer.Start();
```
5. Create a `LoadTestsSilent()` method that silently re-queries lab tests from the database (same query as `LoadTests()`) but does NOT show any status message and only updates the collection if the count changed (to prevent UI flickering):
```csharp
private void LoadTestsSilent()
{
    try
    {
        var tests = _db.LabTests
            .Include(t => t.Patient)
            .Include(t => t.RequestedBy)
            .Where(t => t.Status != LabTestStatus.Completed)
            .OrderByDescending(t => t.RequestedAt)
            .ToList();

        if (RequestedTests.Count != tests.Count)
        {
            RequestedTests = new ObservableCollection<LabTest>(tests);
            HasNoTests = RequestedTests.Count == 0;
        }
    }
    catch (Exception) { /* Fail silently */ }
}
```
6. **Delete** the `RefreshTests()` relay command method entirely
7. Open `LabPanelView.axaml` and **remove** the refresh button (`تحديث`) from the header (line 17)
8. Also remove the local `_statusMessage` field if it shadows `ViewModelBase.StatusMessage`. Use `ShowError()` and `ShowSuccess()` from the base class instead.

---

## 🆕 Task 4: Add Auto-Polling to Cashier Panel & Remove Refresh Button

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours  
**Files:**
- `/Features/Cashier/ViewModels/CashierPanelViewModel.cs`
- `/Features/Cashier/Views/CashierPanelView.axaml`

### Instructions:
1. Open `CashierPanelViewModel.cs`
2. Add `using Avalonia.Threading;` at the top
3. Add a `private readonly DispatcherTimer _refreshTimer;` field
4. In the constructor, initialize the timer to poll every 5 seconds:
```csharp
_refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
_refreshTimer.Tick += (s, e) => LoadInvoicesSilent();
_refreshTimer.Start();
```
5. Create a `LoadInvoicesSilent()` method that silently re-queries pending invoices from the database (same query as `LoadInvoices()`) but does NOT show a status message and only updates the collection if the count changed:
```csharp
private void LoadInvoicesSilent()
{
    try
    {
        var invoices = _db.Invoices
            .Include(i => i.Patient)
            .Where(i => i.Status == InvoiceStatus.Pending)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();

        if (PendingInvoices.Count != invoices.Count)
        {
            PendingInvoices = new ObservableCollection<Invoice>(invoices);
            HasNoInvoices = PendingInvoices.Count == 0;
        }
    }
    catch (Exception) { /* Fail silently */ }
}
```
6. **Delete** the `RefreshInvoices()` relay command method entirely
7. Open `CashierPanelView.axaml` and **remove** the refresh button (`تحديث`) from the header (line 17)
8. Also remove the local `_statusMessage` field if it shadows `ViewModelBase.StatusMessage`. Use `ShowError()` and `ShowSuccess()` from the base class instead.

---

## 📁 Your Files

```
/Infrastructure/
├── GlobalExceptionHandler.cs     ← Old
└── Converters/
    └── TicketStatusConverter.cs  ← Old

/Features/Lab/
├── ViewModels/
│   └── LabPanelViewModel.cs     ← NEW Edit
└── Views/
    └── LabPanelView.axaml       ← NEW Edit

/Features/Cashier/
├── ViewModels/
│   └── CashierPanelViewModel.cs ← NEW Edit
└── Views/
    └── CashierPanelView.axaml   ← NEW Edit

Program.cs                        ← Old
App.axaml                         ← Old
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/lab-cashier-polling` |
| **Work Directory** | `/Features/Lab` + `/Features/Cashier` |
| **Merge To** | `main` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout -b feature/lab-cashier-polling
git pull origin main
git add .
git commit -m "feat(lab+cashier): add auto-polling + remove refresh buttons"
git push origin feature/lab-cashier-polling
```

---

## 🧹 Code Formatting Rule (Mandatory)

Before **every** `git push`, you MUST run:

```bash
dotnet format HealthCenter.Desktop.csproj
```

```bash
# ✅ Full workflow before pushing:
dotnet format HealthCenter.Desktop.csproj
git add .
git commit -m "your message"
git push origin feature/lab-cashier-polling
```

---

**Questions?** Ask Hassan (Team Lead)
