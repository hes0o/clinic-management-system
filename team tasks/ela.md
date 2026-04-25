# 👨‍⚕️ Ela - Doctor Feature Engineer
## إيلا - مهندس ميزة الطبيب

---

## 📋 Role Overview

**English:** This sprint you are fixing the disappearing patients bug (Issue #34) caused by mismatched ticket statuses between the Nurse and Doctor panels.

**Arabic:** مهمتك في هذا السبرينت هي إصلاح مشكلة اختفاء المرضى (مشكلة #34) الناتجة عن عدم تطابق حالات التذاكر بين لوحتي التمريض والطبيب.

---


## 🆕 Assigned Issues

### ✅ Task: fix ui : make the ui focus on the core of the page (closes #36)
**Description:**
No description provided.

**Instructions:**
1. Review the issue details.
2. Fix the related components.
3. Make sure to commit with `closes #36` so it links to the GitHub issue.

---

### ✅ Task: Disappearing Patients (closes #34)
**Description:**
When adding more than patient to the call after the reception it goes to the nurse then to doctor and appears then disappears.
The root cause is that when a patient is sent from Reception to the Nurse, their status is `Waiting`. The Nurse then checks the vitals and changes the status to `ReadyForDoctor`. However, the Doctor panel's `LoadQueueSilent()` method is currently filtering the queue for `TicketStatus.Waiting`. As a result, the patient vanishes from the system.

**Instructions:**
1. Review the issue details.
2. Modify `DoctorPanelViewModel.cs` (lines 251 & 281) to include `TicketStatus.ReadyForDoctor` in its query condition so doctors can see patients sent by nurses.
3. Make sure to commit with `closes #34` so it links to the GitHub issue.

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
