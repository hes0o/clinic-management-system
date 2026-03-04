# 🏨 Wissam - Reception & Nurse Feature Engineer
## وسام - مهندس ميزة الاستقبال والتمريض

---

## 📋 Role Overview

**English:** This sprint your job is to fix two issues across the Nurse panel and the Reception panel: (1) stale vitals form data persists when the nurse selects a different patient, and (2) raw status text and non-user-friendly error presentation needs improving in both panels.

**Arabic:** مهمتك في هذا السبرينت هي إصلاح مشكلتين في لوحة التمريض ولوحة الاستقبال: (1) بيانات نموذج العلامات الحيوية تبقى عند اختيار مريض مختلف، (2) تحسين عرض حالة التذاكر ورسائل الخطأ.

---

## 🐛 Bugs You Are Fixing

### Bug 2 — Stale Vitals Data in Nurse Panel
**Location:** `NursePanelViewModel.cs` — `SelectedTicket` property change  
**Problem:** When the nurse selects a different patient from the queue list, the form fields (BloodPressure, Temperature, HeartRate, Weight) still show the data she typed for the **previous** patient. She may accidentally save the wrong patient's vitals.

### Bug 1 — Status Message Color in Nurse Panel
**Location:** `NursePanelView.axaml`  
**Problem:** The `StatusMessage` text is always shown in green (`#16A34A`), even when it's an error message (e.g., "لا يوجد طبيب مسجّل في النظام." is shown green instead of red). Error messages must be red and success messages must be green.

---

## ✅ Task 1: Clear Vitals Form When a New Patient Is Selected (Nurse Panel)

**Priority:** 🔴 High | **Estimated Time:** 1 hour  
**File:** `/Features/Nurse/ViewModels/NursePanelViewModel.cs`

### Instructions:
1. Open `NursePanelViewModel.cs`
2. Add a `partial void OnSelectedTicketChanged(QueueTicket? value)` method
3. Inside it, clear all vitals fields when a new ticket is selected:
   - `BloodPressure = string.Empty`
   - `Temperature = string.Empty`
   - `HeartRate = string.Empty`
   - `Weight = string.Empty`
   - Also clear the `StatusMessage` so previous feedback does not confuse the nurse

### Code Example:
```csharp
// This is called automatically by CommunityToolkit.Mvvm when SelectedTicket changes
partial void OnSelectedTicketChanged(QueueTicket? value)
{
    // Clear the form so previous patient's data doesn't contaminate
    BloodPressure = string.Empty;
    Temperature = string.Empty;
    HeartRate = string.Empty;
    Weight = string.Empty;
    StatusMessage = string.Empty; // clear previous status
    IsError = false;
}
```

---

## ✅ Task 2: Fix Error/Success Display in Nurse Panel

**Priority:** 🔴 High | **Estimated Time:** 45 minutes  
**Files:**
- `/Features/Nurse/ViewModels/NursePanelViewModel.cs`
- `/Features/Nurse/Views/NursePanelView.axaml`

### Instructions (ViewModel):
1. Make sure `NursePanelViewModel` inherits from `ViewModelBase`
2. Replace all direct `StatusMessage = "..."` assignments with:
   - `ShowSuccess("...")` for success cases
   - `ShowError("...")` for error/warning cases

### Instructions (XAML):
Replace the single green `TextBlock` with two separate bordered banners — one for success, one for error.

---

## ✅ Task 3: Fix Raw Status Text in Nurse Queue List

**Priority:** 🟡 Medium | **Estimated Time:** 30 minutes  
**File:** `/Features/Nurse/Views/NursePanelView.axaml`

### Instructions:
Use Ahmed's `TicketStatusConverter` on the status text in the patient list.

---

## ✅ Task 4: Verify Reception View Error Display is Correct

**Priority:** 🟡 Medium | **Estimated Time:** 30 minutes  
**File:** `/Features/Reception/Views/ReceptionView.axaml`

The Reception view already has a good error/success pattern. Verify it works correctly.

---

## 🆕 Task 5: Add Auto-Polling to Reception Panel & Remove Refresh Button

**Priority:** 🔴 High | **Estimated Time:** 1 hour  
**Files:**
- `/Features/Reception/ViewModels/ReceptionViewModel.cs`
- `/Features/Reception/Views/ReceptionView.axaml`

### Instructions:
1. Open `ReceptionViewModel.cs`
2. Add `using Avalonia.Threading;` at the top
3. Add a `private readonly DispatcherTimer _refreshTimer;` field
4. In the constructor, initialize the timer to poll every 5 seconds:
```csharp
_refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
_refreshTimer.Tick += (s, e) => LoadQueueSilent();
_refreshTimer.Start();
```
5. Create a `LoadQueueSilent()` method that silently re-queries the today's queue from the database (same query as `LoadQueue()`) but does NOT show a status message. Only update the `ObservableCollection` if the count changed.
6. Remove the `RefreshQueueCommand` relay command method entirely
7. The refresh button has already been removed from the AXAML — verify it is gone

---

## 📁 Your Files

```
/Features/Nurse/
├── ViewModels/
│   └── NursePanelViewModel.cs   ← Edit
└── Views/
    └── NursePanelView.axaml     ← Edit

/Features/Reception/
├── ViewModels/
│   └── ReceptionViewModel.cs    ← Edit
└── Views/
    └── ReceptionView.axaml      ← Verify
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/reception-polling` |
| **Work Directory** | `/Features/Nurse` + `/Features/Reception` |
| **Merge To** | `main` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout -b feature/reception-polling
git pull origin main
git add .
git commit -m "feat(reception): add auto-polling + remove refresh button"
git push origin feature/reception-polling
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
git push origin feature/reception-polling
```

---

**Questions?** Ask Hassan (Team Lead)
