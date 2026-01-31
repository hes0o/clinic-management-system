# 🏨 Wissam - Reception Feature Engineer
## وسام - مهندس ميزة الاستقبال

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are responsible for the Reception module - patient registration, search functionality, and ticket printing. This is the first screen users see.

**Arabic:** أنت مسؤول عن وحدة الاستقبال - تسجيل المرضى ووظيفة البحث وطباعة التذاكر. هذه أول شاشة يراها المستخدمون.

---

## 🌿 Branch Rules | قواعد الفروع

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/reception` |
| **Work Directory** | `/Features/Reception` folder |
| **Merge To** | `develop` (via PR) |
| **Requires** | Hassan's approval |

### Branch Setup Commands:
```bash
# First time setup
git checkout -b feature/reception
git push -u origin feature/reception

# Daily workflow
git checkout feature/reception
git pull origin develop
# ... do your work ...
git add .
git commit -m "feat(reception): your message here"
git push origin feature/reception
```

---

## ✅ Task 1: Enhanced Patient Registration Form
### المهمة 1: نموذج تسجيل مريض محسّن

**Priority:** 🔴 High | **Estimated Time:** 3 hours
**Files:** 
- `/Features/Reception/Views/ReceptionView.axaml`
- `/Features/Reception/ViewModels/ReceptionViewModel.cs`

#### English Instructions:
1. Create a dedicated Reception folder structure:
   ```
   /Features/Reception/
   ├── Views/
   │   └── ReceptionView.axaml
   │   └── ReceptionView.axaml.cs
   └── ViewModels/
       └── ReceptionViewModel.cs
   ```
2. Add new fields to registration form:
   - Full Name (required) - الاسم الكامل
   - Phone Number (required) - رقم الهاتف
   - Date of Birth (optional) - تاريخ الميلاد
   - Gender dropdown (Male/Female) - الجنس
   - Blood Type dropdown - فصيلة الدم
   - Address (optional) - العنوان
   - Emergency Contact - رقم الطوارئ
3. Add validation:
   - Phone must be 10 digits starting with 05
   - Name must be at least 3 characters
   - Show error messages in Arabic

#### التعليمات بالعربية:
1. أنشئ هيكل مجلد الاستقبال
2. أضف الحقول الجديدة لنموذج التسجيل
3. أضف التحقق من صحة البيانات

#### Code Example (ViewModel):
```csharp
public partial class ReceptionViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddPatientCommand))]
    private string _patientName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddPatientCommand))]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private DateTime? _dateOfBirth;

    [ObservableProperty]
    private Gender? _selectedGender;

    [ObservableProperty]
    private string? _bloodType;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<string> BloodTypes { get; } = new()
    {
        "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
    };

    public ObservableCollection<Gender> Genders { get; } = new()
    {
        Gender.Male, Gender.Female
    };

    private bool CanAddPatient() =>
        !string.IsNullOrWhiteSpace(PatientName) &&
        PatientName.Length >= 3 &&
        PhoneNumber.Length == 10 &&
        PhoneNumber.StartsWith("05");

    [RelayCommand(CanExecute = nameof(CanAddPatient))]
    private void AddPatient()
    {
        // Validation
        if (!PhoneNumber.StartsWith("05") || PhoneNumber.Length != 10)
        {
            ErrorMessage = "رقم الهاتف يجب أن يبدأ بـ 05 ويتكون من 10 أرقام";
            return;
        }
        // ... save patient
    }
}
```

---

## ✅ Task 2: Advanced Search Functionality
### المهمة 2: وظيفة بحث متقدمة

**Priority:** 🟡 Medium | **Estimated Time:** 2 hours

#### English Instructions:
1. Implement multi-criteria search:
   - Search by name (partial match)
   - Search by phone number
   - Search by date registered
2. Add search filters dropdown
3. Show "No results found" message in Arabic
4. Highlight search terms in results

#### التعليمات بالعربية:
1. نفذ البحث بمعايير متعددة
2. أضف قائمة منسدلة لفلاتر البحث
3. اعرض رسالة "لا توجد نتائج" بالعربية
4. ظلل مصطلحات البحث في النتائج

#### Code Example:
```csharp
[RelayCommand]
private void Search()
{
    var query = SearchQuery.Trim().ToLower();
    
    var results = _db.Patients
        .Where(p => 
            p.FullName.ToLower().Contains(query) ||
            p.PhoneNumber.Contains(query))
        .OrderByDescending(p => p.CreatedAt)
        .Take(50)
        .ToList();

    if (results.Count == 0)
    {
        StatusMessage = "لا توجد نتائج للبحث";
    }
    
    Patients = new ObservableCollection<Patient>(results);
}
```

---

## ✅ Task 3: Ticket Printing Feature
### المهمة 3: ميزة طباعة التذكرة

**Priority:** 🟡 Medium | **Estimated Time:** 2.5 hours

#### English Instructions:
1. Create a ticket preview dialog
2. Design ticket layout (80mm thermal printer format):
   - Clinic name & logo
   - Ticket number (large, centered)
   - Patient name
   - Date & time
   - Estimated wait count
3. Add print button functionality
4. Show success message after printing

#### التعليمات بالعربية:
1. أنشئ نافذة معاينة التذكرة
2. صمم شكل التذكرة (تنسيق طابعة حرارية 80 مم)
3. أضف وظيفة زر الطباعة
4. اعرض رسالة نجاح بعد الطباعة

#### Ticket Design (ASCII Preview):
```
╔══════════════════════════════╗
║       🏥 المركز الصحي        ║
║══════════════════════════════║
║                              ║
║            ٢٣               ║
║      (رقم التذكرة)          ║
║                              ║
║──────────────────────────────║
║  الاسم: أحمد محمد علي        ║
║  التاريخ: 2026/01/31        ║
║  الوقت: 10:30 ص             ║
║──────────────────────────────║
║  أمامك: 5 مرضى             ║
╚══════════════════════════════╝
```

---

## 📁 Your Files | ملفاتك

```
/Features/Reception/
├── Views/
│   ├── ReceptionView.axaml       ← Create/Edit
│   ├── ReceptionView.axaml.cs    ← Create/Edit
│   └── TicketPreviewDialog.axaml ← Create
└── ViewModels/
    └── ReceptionViewModel.cs     ← Create/Edit
```

---

## ⚠️ Important Notes | ملاحظات مهمة

- All text must be in Arabic (RTL)
- Phone validation: must start with 05, exactly 10 digits
- Always show feedback to user (success/error messages)
- Test with Arabic names that include special characters

**Questions?** Ask Hassan (Team Lead)
