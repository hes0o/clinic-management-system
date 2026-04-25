# 👑 Hassan - Team Lead
## حسن - قائد الفريق

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are the Team Lead. Your job this sprint is to review all PRs for the new Sprint 2 integration features and ensure the full patient flow (Reception → Nurse → Doctor → Lab → Cashier) works end-to-end.

**Arabic:** أنت قائد الفريق. مهمتك في هذا السبرينت هي مراجعة جميع طلبات الدمج للميزات الجديدة والتأكد من أن تدفق المريض الكامل يعمل بشكل صحيح.

---


## 🆕 Assigned Issues

### ✅ Task: Missing Global Exception Handling
**Description:**
Critical database operations like `_db.SaveChanges()` inside `SavePatient` and `IssueTicket` are completely unprotected by `try-catch` blocks. If the database locks up or throws an exception, the entire application will crash.

**Instructions:**
1. Wrap database calls in `ReceptionViewModel.cs` and `CashierPanelViewModel.cs` with robust `try-catch` blocks.
2. Use the `ShowError(ex.Message)` method to gracefully report database failures to the user instead of crashing.

---

### ✅ Task: The initial diagnosis feature (closes #30)
**Description:**
The initial diagnosis feature is currently located in the doctor's dashboard, but it should be on the nurse's screen.

**Instructions:**
1. Review the issue details.
2. Fix the related components.
3. Make sure to commit with `closes #30` so it links to the GitHub issue.

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `main` (direct access) |
| **Protected** | `main` |
| **Merge Authority** | Only YOU can merge to `main` |

---

## ⚠️ Important Notes

- Do NOT implement the features yourself — delegate to the team
- Your job is reviews and merging
- All team members must pull `main` and rebase before opening a PR

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
