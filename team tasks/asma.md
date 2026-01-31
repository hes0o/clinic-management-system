# 🧪 Asma - Testing & Documentation
## أسماء - الاختبار والتوثيق

---

## 📋 Role Overview

**English:** Responsible for testing all features, documenting bugs, and creating user guides.

**Arabic:** مسؤولة عن اختبار جميع الميزات وتوثيق الأخطاء وإنشاء أدلة المستخدم.

---

## 🌿 Branch Rules

| Rule | Description |
|------|-------------|
| **Branch** | `feature/testing` |
| **Directory** | `/docs` folder |
| **Merge To** | `develop` (via PR) |

```bash
git checkout -b feature/testing
git push -u origin feature/testing
```

---

## ✅ Task 1: Feature Testing Checklist

**Priority:** 🔴 High | **Time:** 3 hours

### Test Each Feature:

#### Reception (الاستقبال)
- [ ] Add new patient with valid data
- [ ] Add patient with invalid phone (should fail)
- [ ] Search by name
- [ ] Search by phone
- [ ] Generate queue ticket
- [ ] Generate ticket without selecting patient (should fail)

#### Queue Display (شاشة الانتظار)
- [ ] Shows current ticket number
- [ ] Shows waiting list
- [ ] Updates when new ticket added
- [ ] Clock shows correct time

#### Doctor Panel (لوحة الطبيب)
- [ ] Call next patient
- [ ] Mark patient present/absent
- [ ] Enter diagnosis
- [ ] Complete visit

### Bug Report Format:
```
🐛 Bug #XX
Screen: [Reception/Queue/Doctor]
Steps: 1. ... 2. ... 3. ...
Expected: ...
Actual: ...
Screenshot: [attach]
```

---

## ✅ Task 2: User Manual (Arabic)

**Priority:** 🟡 Medium | **Time:** 2 hours
**File:** `/docs/USER_MANUAL_AR.md`

### Sections to Write:
1. تشغيل التطبيق (Starting the App)
2. تسجيل مريض جديد (Adding a Patient)
3. إنشاء تذكرة انتظار (Creating a Ticket)
4. نداء المريض (Calling a Patient)
5. إنهاء الزيارة (Completing a Visit)

Include screenshots for each step.

---

## ✅ Task 3: Presentation Slides

**Priority:** 🟡 Medium | **Time:** 2 hours

### Prepare:
1. Project overview slides
2. Screenshots of each screen
3. Features list
4. Demo video (optional)
5. Team contributions

---

## 📁 Your Files

```
/docs/
├── USER_MANUAL_AR.md     ← Create
├── BUG_REPORTS.md        ← Create
├── screenshots/          ← Create folder
│   ├── reception.png
│   ├── queue.png
│   └── doctor.png
└── PRESENTATION.pptx     ← Create
```

---

## 🧪 How to Test

```bash
# Run the app
cd /Users/hassanchawa/clinic-management-system
dotnet run

# Test each feature manually
# Document any bugs found
```

**Questions?** Ask Hassan
