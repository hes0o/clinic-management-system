# 📺 Eylaf - Queue Feature Engineer
## إيلاف - مهندس ميزة الطابور

---

## 📋 Role Overview

**English:** Responsible for the Queue Display module - waiting room screen showing ticket numbers.

**Arabic:** مسؤول عن شاشة غرفة الانتظار التي تعرض أرقام التذاكر.

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Branch** | `feature/queue` |
| **Directory** | `/Features/Queue` folder |
| **Merge To** | `develop` (via PR) |

```bash
git checkout -b feature/queue
git push -u origin feature/queue
```

---

## ✅ Task 1: Audio Notifications

**Priority:** 🔴 High | **Time:** 2.5 hours

### Instructions:
1. Add audio when patient is called
2. Use Text-to-Speech: "رقم [NUMBER]"
3. Play chime before announcement
4. Add mute button

---

## ✅ Task 2: Information Display

**Priority:** 🟡 Medium | **Time:** 2 hours

### Instructions:
1. Add clinic hours section
2. Add current date + Hijri date
3. Add health tips carousel
4. Make text large (readable from distance)

---

## ✅ Task 3: Estimated Wait Time

**Priority:** 🟡 Medium | **Time:** 2 hours

### Instructions:
1. Calculate average time per patient
2. Show "أمامك: X مريض"
3. Display estimated minutes
4. Auto-update every 5 seconds

---

## 📁 Your Files

```
/Features/Queue/
├── Views/QueueDisplayView.axaml
├── ViewModels/QueueDisplayViewModel.cs
└── Services/AudioNotificationService.cs (create)
```

**Questions?** Ask Hassan
