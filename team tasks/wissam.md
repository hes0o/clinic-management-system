# 🏨 Wissam - Reception & Nurse Feature Engineer
## وسام - مهندس ميزة الاستقبال والتمريض

---

## 📋 Role Overview

**English:** This sprint your job is to fix hardcoded doctor assignments in the Nurse panel to properly handle clinics with multiple doctors, and resolve other interface problems between the Nurse and Doctor panels.

**Arabic:** مهمتك في هذا السبرينت هي إصلاح التعيين الثابت للطبيب في لوحة التمريض للتعامل بشكل صحيح مع العيادات التي بها عدة أطباء، وحل مشاكل الواجهة الأخرى.

---


## 🆕 Assigned Issues

### ✅ Task: fix the hover scroll on medicine selection (closes #37)
**Description:**
No description provided.

**Instructions:**
1. Review the issue details.
2. Fix the related components.
3. Make sure to commit with `closes #37` so it links to the GitHub issue.

---

### ✅ Task: nurse and doctor interfaces problems (closes #35)
**Description:**
No description provided.

**Instructions:**
1. Review the issue details.
2. Fix the related components.
3. Make sure to commit with `closes #35` so it links to the GitHub issue.

---

### ✅ Task: Hardcoded Single Doctor Assumption
**Description:**
When the nurse saves vitals, the code automatically assigns the visit to the first doctor it finds: `_db.Users.FirstOrDefault(u => u.Role == UserRole.Doctor)`. This means if a clinic has two doctors, the second doctor will never receive patients.

**Instructions:**
1. Add a Dropdown (ComboBox) to `NursePanelView.axaml` allowing the nurse to select which doctor to send the patient to.
2. Update `NursePanelViewModel.cs` to assign the `Visit.DoctorId` to the selected doctor instead of relying on `FirstOrDefault`.
3. Commit with an appropriate message.

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
