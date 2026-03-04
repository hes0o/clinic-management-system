# 🗄️ Bassam - Database Engineer
## بسام - مهندس قواعد البيانات

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** This sprint your job is to enhance the patient history feature in the Doctor panel. You will update the database queries to include lab tests and doctor information, and build a detailed history UI so the doctor can see a patient's full medical background.

**Arabic:** مهمتك في هذا السبرينت هي تحسين ميزة سجل المريض في لوحة الطبيب. ستقوم بتحديث استعلامات قاعدة البيانات لتشمل نتائج المختبر ومعلومات الطبيب، وبناء واجهة سجل مفصلة.

---

## 🆕 Task 1: Enhance Doctor Patient History View

**Priority:** 🔴 High | **Estimated Time:** 2 hours  
**Files:**
- `/Database/Entities/Visit.cs` — review structure
- `/Features/Doctor/ViewModels/DoctorPanelViewModel.cs` — enhance `LoadPatientHistory()`
- `/Features/Doctor/Views/DoctorPanelView.axaml` — enhance history UI

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
│   └── Visit.cs               ← Review
/Features/Doctor/
├── ViewModels/
│   └── DoctorPanelViewModel.cs ← Edit (LoadPatientHistory)
└── Views/
    └── DoctorPanelView.axaml   ← Edit (History UI)
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
