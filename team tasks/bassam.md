# 🗄️ Bassam - Database Engineer
## بسام - مهندس قواعد البيانات

---

## 📋 Role Overview | نظرة عامة على الدور

**English:** You are responsible for all database-related work including Entity models, migrations, and data seeding. You work exclusively in the `/Database` folder.

**Arabic:** أنت مسؤول عن جميع أعمال قاعدة البيانات بما في ذلك نماذج الكيانات والتهجيرات وإدخال البيانات التجريبية. تعمل حصرياً في مجلد `/Database`.

---

## 🌿 Branch Rules | قواعد الفروع

| Rule | Description |
|------|-------------|
| **Your Branch** | `feature/database` |
| **Work Directory** | `/Database` folder ONLY |
| **Merge To** | `develop` (via PR) |
| **Requires** | Hassan's approval |

### Branch Setup Commands:
```bash
# First time setup
git checkout -b feature/database
git push -u origin feature/database

# Daily workflow
git checkout feature/database
git pull origin develop  # Get latest changes
# ... do your work ...
git add .
git commit -m "feat(db): your message here"
git push origin feature/database
```

---

## ✅ Task 1: Enhance Patient Entity
### المهمة 1: تحسين كيان المريض

**Priority:** 🔴 High | **Estimated Time:** 2 hours
**File:** `/Database/Entities/Patient.cs`

#### English Instructions:
1. Open `/Database/Entities/Patient.cs`
2. Add the following new properties:
   - `DateOfBirth` (DateTime?) - تاريخ الميلاد
   - `Gender` (enum: Male/Female) - الجنس
   - `Address` (string?) - العنوان
   - `BloodType` (string?) - فصيلة الدم
   - `EmergencyContact` (string?) - رقم الطوارئ
   - `Notes` (string?) - ملاحظات
3. Add data annotations for validation
4. Test that the app still builds

#### التعليمات بالعربية:
1. افتح ملف `/Database/Entities/Patient.cs`
2. أضف الخصائص الجديدة المذكورة أعلاه
3. أضف تعليقات التحقق من البيانات
4. تأكد أن التطبيق يعمل بعد التغييرات

#### Code Example:
```csharp
public class Patient
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    // ADD THESE NEW FIELDS:
    public DateTime? DateOfBirth { get; set; }
    
    public Gender? Gender { get; set; }
    
    [MaxLength(200)]
    public string? Address { get; set; }
    
    [MaxLength(5)]
    public string? BloodType { get; set; }
    
    [Phone]
    public string? EmergencyContact { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum Gender
{
    Male,    // ذكر
    Female   // أنثى
}
```

---

## ✅ Task 2: Create Seed Data
### المهمة 2: إنشاء بيانات تجريبية

**Priority:** 🟡 Medium | **Estimated Time:** 1.5 hours
**File:** `/Database/HealthCenterDbContext.cs`

#### English Instructions:
1. Open `HealthCenterDbContext.cs`
2. In the `OnModelCreating` method, add seed data
3. Add at least 10 sample patients with Arabic names
4. Add 5 sample queue tickets for today
5. Add 3 sample visits

#### التعليمات بالعربية:
1. افتح ملف `HealthCenterDbContext.cs`
2. في دالة `OnModelCreating` أضف البيانات التجريبية
3. أضف 10 مرضى تجريبيين بأسماء عربية
4. أضف 5 تذاكر انتظار لليوم
5. أضف 3 زيارات تجريبية

#### Code Example:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Seed Patients
    modelBuilder.Entity<Patient>().HasData(
        new Patient 
        { 
            Id = Guid.NewGuid(), 
            FullName = "أحمد محمد علي", 
            PhoneNumber = "0501234567",
            Gender = Gender.Male,
            BloodType = "A+"
        },
        new Patient 
        { 
            Id = Guid.NewGuid(), 
            FullName = "فاطمة خالد العمري", 
            PhoneNumber = "0559876543",
            Gender = Gender.Female,
            BloodType = "O-"
        }
        // Add 8 more...
    );
}
```

---

## ✅ Task 3: Add EF Core Migrations Support
### المهمة 3: إضافة دعم التهجيرات

**Priority:** 🟡 Medium | **Estimated Time:** 1 hour

#### English Instructions:
1. Install EF Core Tools if not installed
2. Add Design package to project
3. Create initial migration
4. Document migration commands for team

#### التعليمات بالعربية:
1. ثبت أدوات EF Core إذا لم تكن مثبتة
2. أضف حزمة التصميم للمشروع
3. أنشئ التهجير الأولي
4. وثق أوامر التهجير للفريق

#### Commands:
```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Add Design package (run in project folder)
dotnet add package Microsoft.EntityFrameworkCore.Design

# Create migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update

# Remove last migration (if needed)
dotnet ef migrations remove
```

---

## 📁 Your Files | ملفاتك

```
/Database
├── Entities/
│   ├── Patient.cs      ← Edit this
│   ├── Appointment.cs  ← Edit this
│   ├── QueueTicket.cs  ← Edit this
│   ├── Visit.cs        ← Edit this
│   └── User.cs         ← Edit this
├── HealthCenterDbContext.cs  ← Edit this
└── Migrations/         ← Will be created
```

---

## ⚠️ Important Notes | ملاحظات مهمة

- ❌ Do NOT edit files outside `/Database` folder
- ✅ Always test with `dotnet build` before committing
- ✅ Use nullable types (`?`) for optional fields
- ✅ Add `[Required]` attribute for mandatory fields
- ✅ Commit messages must start with `feat(db):` or `fix(db):`

**Questions?** Ask Hassan (Team Lead)
