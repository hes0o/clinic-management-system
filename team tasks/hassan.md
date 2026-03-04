# 👑 Hassan - Team Lead
## حسن - قائد الفريق

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are the Team Lead. Your job this sprint is to review all PRs for the new Sprint 2 integration features and ensure the full patient flow (Reception → Nurse → Doctor → Lab → Cashier) works end-to-end.

**Arabic:** أنت قائد الفريق. مهمتك في هذا السبرينت هي مراجعة جميع طلبات الدمج للميزات الجديدة والتأكد من أن تدفق المريض الكامل يعمل بشكل صحيح.

---

## 🆕 Task 1: Code Review & Merge for Sprint 2 Integration Tasks

**Priority:** 🔴 High | **Estimated Time:** ongoing

### Instructions:
1. Review all new PRs from the team for Sprint 2 integration features
2. Verify that the Doctor→Lab→Cashier flow works end-to-end
3. Test that auto-polling is working correctly on Lab and Cashier pages
4. Ensure all refresh buttons have been removed from Lab and Cashier views
5. Validate that the Doctor can properly send patients to Lab and select tests
6. Validate that completed visits generate invoices for the Cashier
7. Merge approved PRs to `main`

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
