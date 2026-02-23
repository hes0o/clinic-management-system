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

> **Note:** `ObservableProperty` automatically generates the `OnSelectedTicketChanged` partial method. You just need to implement it.

---

## ✅ Task 2: Fix Error/Success Display in Nurse Panel

**Priority:** 🔴 High | **Estimated Time:** 45 minutes  
**Files:**
- `/Features/Nurse/ViewModels/NursePanelViewModel.cs`
- `/Features/Nurse/Views/NursePanelView.axaml`

### Instructions (ViewModel):
1. Make sure `NursePanelViewModel` inherits from `ViewModelBase` (pull `develop` to get Hassan's updated base class with `ShowError`/`ShowSuccess`/`IsError`)
2. Replace all direct `StatusMessage = "..."` assignments with:
   - `ShowSuccess("...")` for success cases
   - `ShowError("...")` for error/warning cases

### What to change in `SaveVitals()`:
```csharp
// BEFORE:
StatusMessage = "الرجاء تحديد مريض من القائمة أولاً.";
// AFTER:
ShowError("الرجاء تحديد مريض من القائمة أولاً.");

// BEFORE:
StatusMessage = "لا يوجد طبيب مسجّل في النظام.";
// AFTER:
ShowError("لا يوجد طبيب مسجّل في النظام.");

// BEFORE:
StatusMessage = $"تم حفظ العلامات الحيوية للمريض: {SelectedTicket.Patient?.FullName}";
// AFTER:
ShowSuccess($"تم حفظ العلامات الحيوية للمريض: {SelectedTicket.Patient?.FullName}");

// In RefreshQueue():
// BEFORE:
StatusMessage = "تم تحديث قائمة المرضى.";
// AFTER:
ShowSuccess("تم تحديث قائمة المرضى.");
```

### Instructions (XAML):
Replace the single green `TextBlock` with two separate bordered banners — one for success, one for error (same pattern as Reception view):

```xml
<!-- Success Banner -->
<Border IsVisible="{Binding StatusMessage, Converter={x:Static StringConverters.IsNotNullOrEmpty}}"
        CornerRadius="6" Padding="12,8" Margin="0,0,0,8"
        Background="#F0FDF4"
        IsVisible="{Binding IsError, Converter={x:Static BoolConverters.Not}}">
    <TextBlock Text="{Binding StatusMessage}"
               Foreground="#15803D" TextWrapping="Wrap" FontWeight="Medium"/>
</Border>

<!-- Error Banner -->
<Border CornerRadius="6" Padding="12,8" Margin="0,0,0,8"
        Background="#FEF2F2"
        IsVisible="{Binding IsError}">
    <TextBlock Text="{Binding StatusMessage}"
               Foreground="#DC2626" TextWrapping="Wrap" FontWeight="Medium"/>
</Border>
```

Place these banners just **above** the vitals fields grid (after the patient name label).

---

## ✅ Task 3: Fix Raw Status Text in Nurse Queue List

**Priority:** 🟡 Medium | **Estimated Time:** 30 minutes  
**File:** `/Features/Nurse/Views/NursePanelView.axaml`

### Instructions:
Coordinate with Ahmed — he will create a `TicketStatusConverter`. Once it's available, use it on the status text in the patient list:

```xml
<!-- BEFORE: -->
<TextBlock Text="{Binding Status}" FontSize="12" Foreground="#94A3B8"/>

<!-- AFTER: -->
<TextBlock Text="{Binding Status, Converter={StaticResource TicketStatusConverter}}"
           FontSize="12" Foreground="#94A3B8"/>
```

---

## ✅ Task 4: Verify Reception View Error Display is Correct

**Priority:** 🟡 Medium | **Estimated Time:** 30 minutes  
**File:** `/Features/Reception/Views/ReceptionView.axaml`

The Reception view already has a good error/success pattern. Your job is to:
1. Verify the success border (`Background="#F0FDF4"`) has `IsVisible` bound to both `StatusMessage IsNotNullOrEmpty` AND `IsError = false` (right now it always shows when there's a message, even if it's an error)
2. Compare with the current AXAML and fix if needed:
```xml
<!-- Success border should only show when NOT an error: -->
<Border IsVisible="{Binding IsError, Converter={x:Static BoolConverters.Not}}"
        ...>
```

---

## 📁 Your Files

```
/Features/Nurse/
├── ViewModels/
│   └── NursePanelViewModel.cs   ← Edit
└── Views/
    └── NursePanelView.axaml     ← Edit

/Features/Reception/
└── Views/
    └── ReceptionView.axaml      ← Verify/Edit
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/reception` |
| **Work Directory** | `/Features/Nurse` + `/Features/Reception` |
| **Merge To** | `develop` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout feature/reception
git pull origin develop
git add .
git commit -m "fix(nurse): clear vitals on patient switch + fix error/success display"
git push origin feature/reception
```

---

## ⚠️ Important Notes

- **Pull `develop` first** to get Hassan's `ViewModelBase` with `ShowError`/`ShowSuccess`/`IsError`
- **Test Task 1** by: selecting patient A → typing vitals → clicking a different patient → verify all fields are empty
- **Test Task 2** by: clicking "حفظ" with no patient selected → verify message is RED not green
- Do not modify files outside `/Features/Nurse` and `/Features/Reception`

**Questions?** Ask Hassan (Team Lead)
