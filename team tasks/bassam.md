# 🗄️ Bassam - Database Engineer
## بسام - مهندس قواعد البيانات

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** This sprint your job is to optimize database queries across the application to reduce Entity Framework memory bloat by adding `.AsNoTracking()` to read-only queries.

**Arabic:** مهمتك في هذا السبرينت هي تحسين استعلامات قاعدة البيانات عبر التطبيق لتقليل استهلاك الذاكرة في Entity Framework بإضافة `.AsNoTracking()` للاستعلامات المخصصة للقراءة فقط.

---


## 🆕 Assigned Issues

### ✅ Task: Entity Framework Memory Bloat
**Description:**
The application fetches many read-only lists (e.g., searching for 50 patients in Reception, or pulling 10 previous visits for History). Because `.AsNoTracking()` is missing, Entity Framework tracks all these objects in memory, drastically increasing RAM usage over time.

**Instructions:**
1. Add `.AsNoTracking()` to all `ToList()` queries across the ViewModels (`ReceptionViewModel`, `DoctorPanelViewModel`, etc.) where the retrieved data is only being read and displayed.
2. Ensure you test the changes to confirm that tracking is not actually needed in those specific instances.
3. Commit with an appropriate message.

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
