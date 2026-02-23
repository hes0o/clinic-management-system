# 👨‍⚕️ Ela - Doctor Feature Engineer
## إيلا - مهندس ميزة الطبيب

---

## 📋 Role Overview

**English:** This sprint you are fixing two bugs in the Doctor panel: (1) the diagnosis form shows stale data from the previous patient after "Call Next" is pressed, and (2) no error/success feedback is shown to the doctor when actions complete or fail.

**Arabic:** مهمتك في هذا السبرينت هي إصلاح خطأين في لوحة الطبيب: (1) نموذج التشخيص يُبقي بيانات المريض السابق بعد الضغط على "نداء التالي"، (2) لا توجد رسائل نجاح أو خطأ للطبيب عند تنفيذ الأوامر.

---

## 🐛 Bugs You Are Fixing

### Bug 2 — Stale Patient Data After "Call Next"
**Location:** `DoctorPanelViewModel.cs` — `CallNext()` and `CallSpecific()` methods  
**Problem:** When a new patient is called, the diagnosis form fields (Diagnosis, Prescriptions, Notes, BloodPressure, Temperature, HeartRate, Weight, SelectedDiagnosis, SelectedMedication) still contain the **previous patient's data**. The doctor may accidentally save the wrong diagnosis for the new patient.

### Bug 1 (partial) — No Error/Success Feedback in Doctor Panel
**Location:** `DoctorPanelView.axaml`  
**Problem:** There is no status message banner in the Doctor panel UI. When "انهاء الزيارة وحفظ" is pressed there is no visual confirmation. If something fails, the doctor sees nothing.

---

## ✅ Task 1: Clear Form When New Patient Is Called

**Priority:** 🔴 High | **Estimated Time:** 1 hour  
**File:** `/Features/Doctor/ViewModels/DoctorPanelViewModel.cs`

### Instructions:
1. Open `DoctorPanelViewModel.cs`
2. Create a private helper method `ClearDiagnosisForm()` that resets all form fields:
   - `Diagnosis = string.Empty`
   - `Prescriptions = string.Empty`
   - `Notes = string.Empty`
   - `BloodPressure = string.Empty`
   - `Temperature = null`
   - `HeartRate = null`
   - `Weight = null`
   - `SelectedDiagnosis = null`
   - `SelectedMedication = null`
3. Call `ClearDiagnosisForm()` at the **start** of `CallNext()` and `CallSpecific()` — **before** saving/updating the previous patient's status

### Code Example:
```csharp
private void ClearDiagnosisForm()
{
    Diagnosis = string.Empty;
    Prescriptions = string.Empty;
    Notes = string.Empty;
    BloodPressure = string.Empty;
    Temperature = null;
    HeartRate = null;
    Weight = null;
    SelectedDiagnosis = null;
    SelectedMedication = null;
}

[RelayCommand]
private void CallNext()
{
    if (WaitingPatients.Count == 0) return;

    ClearDiagnosisForm(); // ← ADD THIS FIRST

    if (CurrentPatient != null && CurrentPatient.Status == TicketStatus.Called)
    {
        CurrentPatient.Status = TicketStatus.AwaitingRecall;
        CurrentPatient.CallCount++;
        _db.SaveChanges();
    }

    var next = WaitingPatients.FirstOrDefault();
    if (next == null) return;

    next.Status = TicketStatus.Called;
    next.CalledAt = DateTime.Now;
    next.CallCount++;
    _db.SaveChanges();

    LoadQueue();
}
```

Apply the same `ClearDiagnosisForm()` call at the start of `CallSpecific()` too.

---

## ✅ Task 2: Add Error/Success Banner to Doctor Panel UI

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours  
**Files:**
- `/Features/Doctor/ViewModels/DoctorPanelViewModel.cs`
- `/Features/Doctor/Views/DoctorPanelView.axaml`

### Instructions (ViewModel):
1. Make `DoctorPanelViewModel` inherit from `ViewModelBase` (Hassan will have added `ShowError` / `ShowSuccess` / `ClearStatus` methods)
2. In `CompleteVisit()`, call `ShowSuccess("تمت زيارة المريض بنجاح وتم الحفظ.")` at the end
3. If the visit save fails (wrap in try/catch), call `ShowError("حدث خطأ أثناء الحفظ. يرجى المحاولة مجدداً.")`
4. In `CallNext()` and `CallSpecific()`, call `ClearStatus()` so the previous message disappears

### Code Example (ViewModel):
```csharp
[RelayCommand]
private void CompleteVisit()
{
    if (CurrentPatient == null) return;

    try
    {
        // ... existing save logic ...
        _db.SaveChanges();
        ClearDiagnosisForm();
        LoadQueue();
        LoadStatistics();
        ShowSuccess("تمت زيارة المريض بنجاح وتم الحفظ.");
    }
    catch (Exception)
    {
        ShowError("حدث خطأ أثناء الحفظ. يرجى المحاولة مجدداً.");
    }
}
```

### Instructions (XAML):
Add a status banner **above the diagnosis form** (between the Current Patient card and the Diagnosis form). Use the same pattern as Reception:

```xml
<!-- Success Banner -->
<Border IsVisible="{Binding StatusMessage, Converter={x:Static StringConverters.IsNotNullOrEmpty}}"
        CornerRadius="8" Padding="14,10" Margin="0,0,0,12"
        Background="{Binding IsError, Converter={x:Static BoolConverters.Not},
                     ConverterParameter='#F0FDF4|#FEF2F2'}">
    <TextBlock Text="{Binding StatusMessage}"
               Foreground="{Binding IsError, Converter={x:Static BoolConverters.Not},
                            ConverterParameter='#15803D|#DC2626'}"
               TextWrapping="Wrap" FontWeight="Medium"/>
</Border>
```

> **Simpler approach (if converter chaining is complex):** Use two separate Borders — one for success (IsError = false) and one for error (IsError = true), same as Reception view does it.

---

## ✅ Task 3: Fix Raw Status Text in Waiting List

**Priority:** 🟡 Medium | **Estimated Time:** 30 minutes  
**File:** `/Features/Doctor/Views/DoctorPanelView.axaml`

### Instructions:
Coordinate with Ahmed — he will create a `TicketStatusConverter`. Once it's available, use it in the waiting list item template where `{Binding Status}` currently shows the raw enum name:

```xml
<!-- BEFORE (shows raw enum): -->
<TextBlock Text="{Binding Status}" FontSize="12" Foreground="#94A3B8"/>

<!-- AFTER (shows Arabic): -->
<TextBlock Text="{Binding Status, Converter={StaticResource TicketStatusConverter}}"
           FontSize="12" Foreground="#94A3B8"/>
```

---

## 📁 Your Files

```
/Features/Doctor/
├── ViewModels/
│   └── DoctorPanelViewModel.cs   ← Edit
└── Views/
    └── DoctorPanelView.axaml     ← Edit
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/doctor` |
| **Work Directory** | `/Features/Doctor` folder |
| **Merge To** | `develop` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout feature/doctor
git pull origin develop
git add .
git commit -m "fix(doctor): clear form on call next + add error/success banner"
git push origin feature/doctor
```

---

## ⚠️ Important Notes

- Pull `develop` first to get Hassan's `ViewModelBase` changes before you start
- The `ClearDiagnosisForm()` fix (Task 1) is the highest priority — do it first
- Test by: calling patient A → filling diagnosis → pressing "نداء التالي" → verifying all fields are empty

**Questions?** Ask Hassan (Team Lead)
