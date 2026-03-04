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
   - `ReadyForDoctor` → `"جاهز للطبيب"`
   - `Called` → `"تم النداء"`
   - `InProgress` → `"قيد الفحص"`
   - `AwaitingRecall` → `"بانتظار إعادة النداء"`
   - `Completed` → `"منتهي"`
   - `Present` → `"حاضر"`

---

## ✅ Task 2: Verify No Raw Exception Leaks from DB Layer

**Priority:** 🟡 Medium | **Estimated Time:** 1 hour  
**File:** `/Database/HealthCenterDbContext.cs`

### Instructions:
1. Review any direct DB calls that might throw raw exceptions to the UI
2. Ensure `EnsureCreated()` failures are caught and handled gracefully
3. If there is any `try/catch` missing around migrations or seeding, add it

---

## 🆕 Task 3: Enhance Doctor Patient History View

**Priority:** 🔴 High | **Estimated Time:** 2 hours  
**Files:**
- `/Database/Entities/Visit.cs` — review structure
- `/Features/Doctor/ViewModels/DoctorPanelViewModel.cs` — enhance `LoadPatientHistory()`

### Instructions:
1. Review the `Visit` entity to ensure it has all necessary fields for a rich history view:
   - `VisitDate`, `Diagnosis`, `Prescriptions`, `Notes`
   - `BloodPressure`, `Temperature`, `HeartRate`, `Weight`
   - Navigation properties: `Doctor` (User), `Nurse` (User), `LabTests` (collection)
2. Open `DoctorPanelViewModel.cs` and enhance `LoadPatientHistory()`:
   - Currently it only loads the last 10 visits with basic data
   - **Include** the `Doctor` navigation property so the history shows which doctor saw the patient
   - **Include** the `LabTests` collection so the doctor can see past lab results
3. Add a new observable property `SelectedHistoryVisit` (type `Visit?`) so the doctor can click a past visit and see its full details
4. After the doctor calls a patient (`CallNext()` / `CallSpecific()`), the history should automatically load

### Code Example:
```csharp
private void LoadPatientHistory(Guid patientId)
{
    var history = _db.Visits
        .Include(v => v.Doctor)
        .Include(v => v.LabTests)
        .Where(v => v.PatientId == patientId)
        .OrderByDescending(v => v.VisitDate)
        .Take(10)
        .ToList();

    PatientHistory = new ObservableCollection<Visit>(history);
}
```

5. In `DoctorPanelView.axaml`, enhance the history section to show:
   - Visit date
   - Doctor name
   - Diagnosis
   - Prescriptions
   - Vital signs (if recorded)
   - Any lab tests requested during that visit

---

## 📁 Your Files

```
/Database/
├── Entities/
│   ├── QueueTicket.cs         ← Old
│   └── Visit.cs               ← NEW Review
└── HealthCenterDbContext.cs    ← Old

/Features/Doctor/
├── ViewModels/
│   └── DoctorPanelViewModel.cs ← NEW Edit (LoadPatientHistory)
└── Views/
    └── DoctorPanelView.axaml   ← NEW Edit (History UI)
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/patient-history` |
| **Work Directory** | `/Database` + `/Features/Doctor` (history UI only) |
| **Merge To** | `main` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout -b feature/patient-history
git pull origin main
git add .
git commit -m "feat(doctor): enhance patient history with lab tests and doctor info"
git push origin feature/patient-history
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
git push origin feature/patient-history
```

---

**Questions?** Ask Hassan (Team Lead)
