# 👨‍⚕️ Ela - Doctor Feature Engineer
## إيلا - مهندس ميزة الطبيب

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are responsible for the Doctor module - patient examination workflow, diagnosis entry, prescription management, and visit history.

**Arabic:** أنت مسؤول عن وحدة الطبيب - سير عمل فحص المريض وإدخال التشخيص وإدارة الوصفات وسجل الزيارات.

---

## 🌿 Branch Rules | قواعد الفروع

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/doctor` |
| **Work Directory** | `/Features/Doctor` folder |
| **Merge To** | `develop` (via PR) |
| **Requires** | Hassan's approval |

### Branch Setup Commands:
```bash
# First time setup
git checkout -b feature/doctor
git push -u origin feature/doctor

# Daily workflow
git checkout feature/doctor
git pull origin develop
# ... do your work ...
git add .
git commit -m "feat(doctor): your message here"
git push origin feature/doctor
```

---

## ✅ Task 1: Patient Visit History Panel
### المهمة 1: لوحة سجل زيارات المريض

**Priority:** 🔴 High | **Estimated Time:** 3 hours
**File:** `/Features/Doctor/Views/DoctorPanelView.axaml`

#### English Instructions:
1. Add a collapsible panel showing patient's previous visits
2. Display for each visit:
   - Visit date
   - Diagnosis
   - Prescribed medications
   - Doctor's notes
3. Sort by most recent first
4. Limit to last 10 visits (with "load more" option)

#### التعليمات بالعربية:
1. أضف لوحة قابلة للطي تعرض زيارات المريض السابقة
2. اعرض لكل زيارة: التاريخ، التشخيص، الأدوية، ملاحظات الطبيب
3. رتب من الأحدث للأقدم
4. حدد بـ 10 زيارات مع خيار "تحميل المزيد"

#### Code Example (ViewModel):
```csharp
[ObservableProperty]
private ObservableCollection<Visit> _patientHistory = new();

[ObservableProperty]
private bool _isHistoryExpanded = false;

private void LoadPatientHistory(Guid patientId)
{
    var history = _db.Visits
        .Where(v => v.PatientId == patientId)
        .OrderByDescending(v => v.VisitDate)
        .Take(10)
        .ToList();
    
    PatientHistory = new ObservableCollection<Visit>(history);
}
```

#### XAML Example:
```xml
<!-- Visit History Panel -->
<Expander Header="📋 سجل الزيارات السابقة" IsExpanded="{Binding IsHistoryExpanded}">
    <ItemsControl ItemsSource="{Binding PatientHistory}">
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <Border Classes="card" Margin="0,8">
                    <StackPanel Spacing="8">
                        <TextBlock Text="{Binding VisitDate, StringFormat='yyyy/MM/dd'}"
                                   FontWeight="Bold" Foreground="#3B82F6"/>
                        <TextBlock Text="{Binding Diagnosis}" TextWrapping="Wrap"/>
                        <TextBlock Text="{Binding Prescriptions}" 
                                   Foreground="#64748B" FontSize="13"/>
                    </StackPanel>
                </Border>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</Expander>
```

---

## ✅ Task 2: Enhanced Diagnosis Form
### المهمة 2: نموذج تشخيص محسّن

**Priority:** 🔴 High | **Estimated Time:** 2.5 hours

#### English Instructions:
1. Add common diagnosis dropdown (autocomplete):
   - Common cold (نزلة برد)
   - Flu (إنفلونزا)
   - Headache (صداع)
   - Stomach pain (ألم المعدة)
   - Custom entry option
2. Add vital signs section:
   - Blood pressure
   - Temperature
   - Heart rate
   - Weight
3. Add common medications dropdown with dosage

#### التعليمات بالعربية:
1. أضف قائمة منسدلة للتشخيصات الشائعة
2. أضف قسم العلامات الحيوية
3. أضف قائمة منسدلة للأدوية الشائعة مع الجرعات

#### Code Example:
```csharp
public ObservableCollection<string> CommonDiagnoses { get; } = new()
{
    "نزلة برد",
    "إنفلونزا",
    "صداع",
    "ألم المعدة",
    "التهاب الحلق",
    "ارتفاع ضغط الدم",
    "السكري",
    "أخرى..."
};

public ObservableCollection<string> CommonMedications { get; } = new()
{
    "باراسيتامول 500mg - مرتين يومياً",
    "أموكسيسيلين 500mg - ثلاث مرات يومياً",
    "إيبوبروفين 400mg - عند الحاجة",
    "أوميبرازول 20mg - قبل الفطور"
};

// Vital Signs
[ObservableProperty]
private string _bloodPressure = string.Empty; // e.g., "120/80"

[ObservableProperty]
private decimal? _temperature; // in Celsius

[ObservableProperty]
private int? _heartRate; // BPM

[ObservableProperty]
private decimal? _weight; // in KG
```

---

## ✅ Task 3: Doctor Statistics Dashboard
### المهمة 3: لوحة إحصائيات الطبيب

**Priority:** 🟡 Medium | **Estimated Time:** 2 hours

#### English Instructions:
1. Add statistics section showing:
   - Patients today
   - Patients this week
   - Patients this month
   - Most common diagnoses (top 5)
2. Display as cards with icons
3. Refresh on demand

#### التعليمات بالعربية:
1. أضف قسم إحصائيات يعرض:
   - مرضى اليوم
   - مرضى هذا الأسبوع
   - مرضى هذا الشهر
   - التشخيصات الأكثر شيوعاً
2. اعرض كبطاقات مع أيقونات
3. تحديث عند الطلب

#### Code Example:
```csharp
[ObservableProperty]
private int _todayPatients;

[ObservableProperty]
private int _weekPatients;

[ObservableProperty]
private int _monthPatients;

private void LoadStatistics()
{
    var today = DateTime.Today;
    var weekStart = today.AddDays(-(int)today.DayOfWeek);
    var monthStart = new DateTime(today.Year, today.Month, 1);
    
    TodayPatients = _db.Visits.Count(v => v.VisitDate.Date == today);
    WeekPatients = _db.Visits.Count(v => v.VisitDate >= weekStart);
    MonthPatients = _db.Visits.Count(v => v.VisitDate >= monthStart);
}
```

---

## 📁 Your Files | ملفاتك

```
/Features/Doctor/
├── Views/
│   ├── DoctorPanelView.axaml       ← Edit
│   ├── DoctorPanelView.axaml.cs    ← Edit
│   └── PatientHistoryPanel.axaml   ← Create
└── ViewModels/
    └── DoctorPanelViewModel.cs     ← Edit
```

---

## ⚠️ Important Notes | ملاحظات مهمة

- Medical data is sensitive - validate all entries
- Use proper units (°C for temperature, mmHg for BP)
- Always show previous allergies if available
- Confirm before completing a visit (no accidental submissions)

**Questions?** Ask Hassan (Team Lead)
