# 👑 Hassan - Team Lead
## حسن - قائد الفريق

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are the Team Lead. Your job this sprint is to coordinate the two bug fixes, review all PRs, and implement the global infrastructure for user-friendly error handling so the rest of the team can use it.

**Arabic:** أنت قائد الفريق. مهمتك في هذا السبرينت هي تنسيق إصلاح الخطأين، مراجعة جميع طلبات الدمج، وتطبيق البنية التحتية لعرض رسائل الخطأ الودية حتى يتمكن باقي الفريق من استخدامها.

---

## 🐛 Background: The Two Bugs We Are Fixing

### Bug 1 — Raw/Code Error Messages
In some interfaces (especially Nurse and Doctor panels), errors are shown either as raw exception text, raw enum values (e.g., `Waiting`, `Called`), or not shown at all. The UI must show friendly Arabic messages like "حدث خطأ، يرجى المحاولة مرة أخرى" instead of code output.

### Bug 2 — Stale Patient Data After "Call Next"
When the doctor presses "نداء المريض التالي" (Call Next Patient), the new patient's information appears in the header — but the diagnosis form fields (Diagnosis, Prescriptions, Notes, vital signs) still contain the **previous patient's data**. The same issue exists in the Nurse panel when switching selected patients. All form fields must be cleared when a new patient is called/selected.

---

## ✅ Task 1: Create Shared `StatusMessage` Infrastructure

**Priority:** 🔴 High | **Estimated Time:** 1.5 hours
**File:** `/ViewModels/ViewModelBase.cs`

### English Instructions:
1. Open `/ViewModels/ViewModelBase.cs`
2. Add two observable properties that all ViewModels can inherit:
   - `StatusMessage` (string) — Arabic message to display
   - `IsError` (bool) — true = red error style, false = green success style
3. Add two helper methods: `ShowError(string msg)` and `ShowSuccess(string msg)`
4. Add a `ClearStatus()` method that resets both to empty/false
5. Verify that `DoctorPanelViewModel`, `NursePanelViewModel`, and `ReceptionViewModel` all inherit from `ViewModelBase`

### Code Example:
```csharp
public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isError;

    protected void ShowError(string msg)
    {
        StatusMessage = msg;
        IsError = true;
    }

    protected void ShowSuccess(string msg)
    {
        StatusMessage = msg;
        IsError = false;
    }

    protected void ClearStatus()
    {
        StatusMessage = string.Empty;
        IsError = false;
    }
}
```

---

## ✅ Task 2: Code Review & Merge

**Priority:** 🔴 High | **Estimated Time:** ongoing

### Instructions:
1. Review PRs from Bassam, Ahmed, Ela, and Wissam as they submit them
2. Make sure each fix addresses only its assigned bug — no scope creep
3. Check that error messages are in Arabic and not raw English exception text
4. Check that form fields are fully cleared (not partially) when calling next patient
5. Merge approved PRs to `develop`

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `main` (direct access) |
| **Protected** | `main`, `develop` |
| **Merge Authority** | Only YOU can merge to `main` |

---

## ⚠️ Important Notes

- Do NOT implement the UI fixes yourself — delegate to the team
- Your job is the shared base infrastructure (Task 1) and reviews (Task 2)
- All team members must pull `develop` and rebase before opening a PR

## 🧹 Code Formatting Rule (Mandatory)

> **The "Clean Code Enforcer" GitHub Action will reject any push with formatting issues — this applies to everyone including the lead.**

Before **every** `git push`, run:

```bash
dotnet format HealthCenter.Desktop.csproj
```

```bash
# ✅ Full workflow before pushing:
dotnet format HealthCenter.Desktop.csproj
git add .
git commit -m "your message"
git push
```

---

**Questions?** You're the lead, figure it out! 😄
