# ⚙️ Ahmed - Infrastructure Engineer
## أحمد - مهندس البنية التحتية

---

## 📋 Role Overview

**English:** This sprint your job is to fix memory leaks caused by `DispatcherTimer` instances across all ViewModels by implementing `IDisposable` in `ViewModelBase`.

**Arabic:** مهمتك في هذا السبرينت هي إصلاح تسرب الذاكرة الناتج عن استخدام `DispatcherTimer` في جميع الـ ViewModels عبر تطبيق `IDisposable` في `ViewModelBase`.

---


## 🆕 Assigned Issues

### ✅ Task: Memory Leaks from DispatcherTimer
**Description:**
The application relies on background polling using `DispatcherTimer` (e.g., refreshing queues every 5 seconds). However, these timers are never stopped when the user leaves the page or when the application cleans up memory, leading to severe memory leaks.

**Instructions:**
1. Make `ViewModelBase` implement `IDisposable` with a virtual `Dispose()` method.
2. Override `Dispose()` in all panel ViewModels (`Reception`, `Nurse`, `Doctor`, `Lab`, `Cashier`) to safely call `_refreshTimer?.Stop()`.
3. Commit with an appropriate message.

## 📁 Your Files

```
/ViewModels/
├── ViewModelBase.cs             ← Edit
/Features/Lab/ViewModels/
├── LabPanelViewModel.cs         ← Edit
/Features/Cashier/ViewModels/
├── CashierPanelViewModel.cs     ← Edit
/Features/Reception/ViewModels/
├── ReceptionViewModel.cs        ← Edit
/Features/Nurse/ViewModels/
├── NursePanelViewModel.cs       ← Edit
/Features/Doctor/ViewModels/
├── DoctorPanelViewModel.cs      ← Edit
```

/Features/Cashier/
├── ViewModels/
│   └── CashierPanelViewModel.cs ← Edit
└── Views/
    └── CashierPanelView.axaml   ← Edit
```

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/lab-cashier-polling` |
| **Work Directory** | `/Features/Lab` + `/Features/Cashier` |
| **Merge To** | `main` (via PR) |
| **Requires** | Hassan's approval |

```bash
git checkout -b feature/lab-cashier-polling
git pull origin main
git add .
git commit -m "feat(lab+cashier): add auto-polling + remove refresh buttons"
git push origin feature/lab-cashier-polling
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
git push origin feature/lab-cashier-polling
```

---

**Questions?** Ask Hassan (Team Lead)
