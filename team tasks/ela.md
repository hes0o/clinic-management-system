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

### Instructions (XAML):
Add a status banner **above the diagnosis form**. Use two separate Borders — one for success (IsError = false) and one for error (IsError = true), same as Nurse view.

---

## ✅ Task 3: Fix Raw Status Text in Waiting List

**Priority:** 🟡 Medium | **Estimated Time:** 30 minutes  
**File:** `/Features/Doctor/Views/DoctorPanelView.axaml`

### Instructions:
Use Ahmed's `TicketStatusConverter` on the status text in the waiting list item template:

```xml
<!-- AFTER (shows Arabic): -->
<TextBlock Text="{Binding Status, Converter={StaticResource TicketStatusConverter}}"
           FontSize="12" Foreground="#94A3B8"/>
```

---

## 🆕 Task 4: Doctor → Lab Integration (Send Patient to Lab & Select Tests)

**Priority:** 🔴 High | **Estimated Time:** 3 hours  
**Files:**
- `/Features/Doctor/ViewModels/DoctorPanelViewModel.cs`
- `/Features/Doctor/Views/DoctorPanelView.axaml`

### Instructions:
1. Add a UI section to the Doctor panel (below the diagnosis form) titled **"طلب فحوصات مخبرية"** (Request Lab Tests)
2. Add a `TextBox` for `SelectedLabTestName` so the doctor can type a test name (e.g., "فحص السكر التراكمي", "فحص CBC")
3. Add a list of common lab tests as quick-select buttons (similar to CommonDiagnoses):
   - `"تحليل دم كامل CBC"`
   - `"فحص السكر التراكمي HbA1c"`
   - `"فحص وظائف الكلى"`
   - `"فحص وظائف الكبد"`
   - `"تحليل بول"`
   - `"فحص الغدة الدرقية TSH"`
4. Add a `[RelayCommand] SendToLab()` method that:
   - Creates a new `LabTest` entity with `Status = LabTestStatus.Requested`
   - Sets `PatientId`, `RequestedById` (current doctor), `VisitId`, `TestName`, `RequestedAt`
   - Saves to DB
   - Shows a success message: `"تم إرسال طلب فحص {TestName} للمختبر"`
5. The Doctor should be able to send **multiple** tests for the same patient before completing the visit

### Code Example:
```csharp
[ObservableProperty] private string _selectedLabTestName = string.Empty;

[RelayCommand]
private void SendToLab()
{
    if (CurrentPatient == null) { ShowError("لا يوجد مريض حالي"); return; }
    if (string.IsNullOrWhiteSpace(SelectedLabTestName)) { ShowError("الرجاء تحديد اسم الفحص"); return; }

    var visit = _db.Visits.FirstOrDefault(v => v.PatientId == CurrentPatient.PatientId && v.VisitDate.Date == DateTime.Today);
    
    var labTest = new LabTest
    {
        PatientId = CurrentPatient.PatientId,
        VisitId = visit?.Id ?? Guid.Empty,
        TestName = SelectedLabTestName,
        Status = LabTestStatus.Requested,
        RequestedById = _db.Users.FirstOrDefault(u => u.Role == UserRole.Doctor)?.Id ?? Guid.Empty,
        RequestedAt = DateTime.UtcNow
    };

    _db.LabTests.Add(labTest);
    _db.SaveChanges();
    ShowSuccess($"تم إرسال طلب فحص: {SelectedLabTestName} للمختبر");
    SelectedLabTestName = string.Empty;
}
```

---

## 🆕 Task 5: Doctor → Cashier Integration (Generate Invoice on Visit Complete)

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours  
**File:** `/Features/Doctor/ViewModels/DoctorPanelViewModel.cs`

### Instructions:
1. In the existing `CompleteVisit()` method, **after** saving the visit and marking the ticket as Completed, automatically create an `Invoice` for the Cashier:
2. Create a new `Invoice` entity with:
   - `VisitId` = the visit just saved
   - `PatientId` = current patient
   - `Amount` = 150.00m (default consultation fee)
   - `TaxAmount` = 22.50m (15% VAT)
   - `Status` = `InvoiceStatus.Pending`
   - `CreatedById` = current doctor's user ID
   - `CreatedAt` = DateTime.UtcNow
3. This invoice will then automatically appear in the Cashier panel through the auto-polling

### Code Example:
```csharp
// Inside CompleteVisit(), after _db.Visits.Add(visit):
var invoice = new Invoice
{
    VisitId = visit.Id,
    PatientId = CurrentPatient.PatientId,
    Amount = 150.00m,
    TaxAmount = 22.50m,
    Status = InvoiceStatus.Pending,
    CreatedById = doctorId,
    CreatedAt = DateTime.UtcNow
};
_db.Invoices.Add(invoice);
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
| **Your Branch** | `feature/doctor-integration` |
| **Work Directory** | `/Features/Doctor` folder |
| **Merge To** | `main` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout -b feature/doctor-integration
git pull origin main
git add .
git commit -m "feat(doctor): add lab test requests + invoice generation"
git push origin feature/doctor-integration
```

---

## 🧹 Code Formatting Rule (Mandatory)

> **A GitHub Action called "Clean Code Enforcer" will automatically reject your push if your code is not properly formatted.**

Before **every** `git push`, you MUST run:

```bash
dotnet format HealthCenter.Desktop.csproj
```

```bash
# ✅ Full workflow before pushing:
dotnet format HealthCenter.Desktop.csproj
git add .
git commit -m "your message"
git push origin feature/doctor-integration
```

---

**Questions?** Ask Hassan (Team Lead)
