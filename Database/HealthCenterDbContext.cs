using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using HealthCenter.Desktop.Database.Entities;

namespace HealthCenter.Desktop.Database;

public class HealthCenterDbContext : DbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<QueueTicket> QueueTickets => Set<QueueTicket>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<LabTest> LabTests => Set<LabTest>();

    private readonly string _dbPath;

    public HealthCenterDbContext()
    {
        // Store database in AppData folder (cross-platform)
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var healthCenterPath = Path.Combine(appDataPath, "HealthCenter");
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(healthCenterPath))
        {
            Directory.CreateDirectory(healthCenterPath);
        }
        
        _dbPath = Path.Combine(healthCenterPath, "healthcenter.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NationalId).HasMaxLength(20);
            entity.HasIndex(e => e.NationalId).IsUnique();
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.FullName);
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.Appointments)
                  .HasForeignKey(e => e.DoctorId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.ScheduledTime);
        });

        // QueueTicket configuration
        modelBuilder.Entity<QueueTicket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.QueueTickets)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.QueueTickets)
                  .HasForeignKey(e => e.DoctorId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Status);
        });

        // Visit configuration
        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Visits)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.Visits)
                  .HasForeignKey(e => e.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Nurse)
                  .WithMany()
                  .HasForeignKey(e => e.NurseId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.VisitDate);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Visit)
                  .WithMany(v => v.Invoices)
                  .HasForeignKey(e => e.VisitId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CreatedBy)
                  .WithMany(u => u.Invoices)
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Status);
        });

        // LabTest configuration
        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Visit)
                  .WithMany(v => v.LabTests)
                  .HasForeignKey(e => e.VisitId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.RequestedBy)
                  .WithMany(u => u.RequestedLabTests)
                  .HasForeignKey(e => e.RequestedById)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.PerformedBy)
                  .WithMany(u => u.PerformedLabTests)
                  .HasForeignKey(e => e.PerformedById)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Status);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Seed Staff Users
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var doctorId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var nurseId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var cashierId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        var labTechId = Guid.Parse("00000000-0000-0000-0000-000000000005");
        var receptionistId = Guid.Parse("00000000-0000-0000-0000-000000000006");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminId,
                Username = "admin",
                PasswordHash = "admin123", // TODO: Hash this properly
                FullName = "مدير النظام",
                Role = UserRole.SuperAdmin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new User
            {
                Id = doctorId,
                Username = "dr_ahmed",
                PasswordHash = "doctor123", 
                FullName = "د. أحمد صالح",
                Role = UserRole.Doctor,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new User
            {
                Id = nurseId,
                Username = "nurse_fatima",
                PasswordHash = "nurse123", 
                FullName = "الممرضة فاطمة",
                Role = UserRole.Nurse,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new User
            {
                Id = cashierId,
                Username = "cashier_omar",
                PasswordHash = "cashier123", 
                FullName = "المحاسب عمر",
                Role = UserRole.Cashier,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new User
            {
                Id = labTechId,
                Username = "tech_salem",
                PasswordHash = "tech123", 
                FullName = "فني المختبر سالم",
                Role = UserRole.LabTechnician,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new User
            {
                Id = receptionistId,
                Username = "rec_nour",
                PasswordHash = "rec123", 
                FullName = "م. الاستقبال نور",
                Role = UserRole.Receptionist,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            }
        );

        // Seed sample patients (10 patients with Arabic names)
        var patient1 = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var patient2 = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var patient3 = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var patient4 = Guid.Parse("10000000-0000-0000-0000-000000000004");
        var patient5 = Guid.Parse("10000000-0000-0000-0000-000000000005");
        var patient6 = Guid.Parse("10000000-0000-0000-0000-000000000006");
        var patient7 = Guid.Parse("10000000-0000-0000-0000-000000000007");
        var patient8 = Guid.Parse("10000000-0000-0000-0000-000000000008");
        var patient9 = Guid.Parse("10000000-0000-0000-0000-000000000009");
        var patient10 = Guid.Parse("10000000-0000-0000-0000-000000000010");

        modelBuilder.Entity<Patient>().HasData(
            new Patient 
            { 
                Id = patient1, FullName = "أحمد محمد علي", PhoneNumber = "0501234567",
                NationalId = "1090123456",
                DateOfBirth = new DateTime(1985, 5, 15), Gender = Gender.Male,
                Address = "الرياض، حي الملز", BloodType = "A+",
                Allergies = "بنسلين",
                ChronicConditions = "ضغط الدم",
                EmergencyContact = "0559876543",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient2, FullName = "فاطمة خالد العمري", PhoneNumber = "0559876543",
                NationalId = "1090234567",
                DateOfBirth = new DateTime(1990, 8, 22), Gender = Gender.Female,
                Address = "جدة، حي الروضة", BloodType = "O-",
                Allergies = "لا يوجد",
                ChronicConditions = "السكري النوع 2",
                EmergencyContact = "0501234567",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient3, FullName = "عبدالله سعد الغامدي", PhoneNumber = "0551112233",
                NationalId = "1090345678",
                DateOfBirth = new DateTime(1978, 3, 10), Gender = Gender.Male,
                Address = "الدمام، حي الفيصلية", BloodType = "B+",
                Allergies = "لا يوجد",
                ChronicConditions = "الربو",
                EmergencyContact = "0554445566",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient4, FullName = "نورة عبدالرحمن القحطاني", PhoneNumber = "0544445566",
                NationalId = "1090456789",
                DateOfBirth = new DateTime(1995, 12, 5), Gender = Gender.Female,
                Address = "مكة المكرمة، العزيزية", BloodType = "AB+",
                Allergies = "الغبار، حبوب اللقاح",
                ChronicConditions = "لا يوجد",
                EmergencyContact = "0551112233",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient5, FullName = "محمد عبدالعزيز الشهري", PhoneNumber = "0567778899",
                NationalId = "1090567890",
                DateOfBirth = new DateTime(1982, 7, 18), Gender = Gender.Male,
                Address = "أبها، حي المنهل", BloodType = "O+",
                Allergies = "لا يوجد",
                ChronicConditions = "ضغط الدم، السكري",
                EmergencyContact = "0561234567",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient6, FullName = "سارة أحمد الدوسري", PhoneNumber = "0533334444",
                NationalId = "1090678901",
                DateOfBirth = new DateTime(1988, 11, 30), Gender = Gender.Female,
                Address = "الرياض، حي النخيل", BloodType = "A-",
                Allergies = "إيبوبروفين",
                ChronicConditions = "لا يوجد",
                InsuranceProvider = "بوبا العربية",
                InsuranceNumber = "INS-2024-001",
                EmergencyContact = "0522223333",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient7, FullName = "خالد فهد المطيري", PhoneNumber = "0522223333",
                NationalId = "1090789012",
                DateOfBirth = new DateTime(1992, 4, 25), Gender = Gender.Male,
                Address = "الطائف، حي الشهداء", BloodType = "B-",
                Allergies = "لا يوجد",
                ChronicConditions = "لا يوجد",
                EmergencyContact = "0533334444",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient8, FullName = "منى سليمان الحربي", PhoneNumber = "0588889999",
                NationalId = "1090890123",
                DateOfBirth = new DateTime(1975, 9, 12), Gender = Gender.Female,
                Address = "المدينة المنورة، حي العزيزية", BloodType = "O+",
                Allergies = "لا يوجد",
                ChronicConditions = "قصور الغدة الدرقية",
                InsuranceProvider = "تأمين المجموعة",
                InsuranceNumber = "INS-2024-002",
                EmergencyContact = "0577778888",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient9, FullName = "ياسر علي الزهراني", PhoneNumber = "0577778888",
                NationalId = "1090901234",
                DateOfBirth = new DateTime(1987, 2, 8), Gender = Gender.Male,
                Address = "جدة، حي السلامة", BloodType = "AB-",
                Allergies = "لا يوجد",
                ChronicConditions = "لا يوجد",
                EmergencyContact = "0588889999",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            },
            new Patient 
            { 
                Id = patient10, FullName = "ريم محمد القرني", PhoneNumber = "0511112222",
                NationalId = "1091012345",
                DateOfBirth = new DateTime(1993, 6, 14), Gender = Gender.Female,
                Address = "الخبر، حي الثقبة", BloodType = "A+",
                Allergies = "لا يوجد",
                ChronicConditions = "لا يوجد",
                EmergencyContact = "0599998888",
                CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2026, 1, 1, 8, 0, 0)
            }
        );

        // Seed sample queue tickets (5 tickets for today)
        var seedDate = new DateTime(2026, 2, 16); // Static date for seed data
        modelBuilder.Entity<QueueTicket>().HasData(
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                TicketNumber = 1,
                PatientId = patient1,
                DoctorId = doctorId,
                Status = TicketStatus.Completed,
                CreatedAt = new DateTime(2026, 2, 16, 8, 0, 0),
                CalledAt = new DateTime(2026, 2, 16, 8, 15, 0),
                CompletedAt = new DateTime(2026, 2, 16, 8, 30, 0),
                CallCount = 1
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                TicketNumber = 2,
                PatientId = patient2,
                DoctorId = doctorId,
                Status = TicketStatus.InProgress,
                CreatedAt = new DateTime(2026, 2, 16, 8, 10, 0),
                CalledAt = new DateTime(2026, 2, 16, 8, 35, 0),
                CallCount = 1
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                TicketNumber = 3,
                PatientId = patient3,
                Status = TicketStatus.Waiting,
                CreatedAt = new DateTime(2026, 2, 16, 8, 20, 0),
                CallCount = 0
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                TicketNumber = 4,
                PatientId = patient4,
                Status = TicketStatus.Waiting,
                CreatedAt = new DateTime(2026, 2, 16, 9, 0, 0),
                CallCount = 0
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                TicketNumber = 5,
                PatientId = patient5,
                Status = TicketStatus.Waiting,
                CreatedAt = new DateTime(2026, 2, 16, 9, 15, 0),
                CallCount = 0
            }
        );

        // Seed sample visits (3 visits)
        var visit1 = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var visit2 = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var visit3 = Guid.Parse("30000000-0000-0000-0000-000000000003");
        
        modelBuilder.Entity<Visit>().HasData(
            new Visit
            {
                Id = visit1,
                PatientId = patient1,
                DoctorId = doctorId,
                NurseId = nurseId,
                Diagnosis = "التهاب الحلق الحاد",
                Prescriptions = "Amoxicillin 500mg - 3 مرات يومياً لمدة 5 أيام",
                Notes = "المريض يعاني من ارتفاع طفيف في درجة الحرارة",
                BloodPressure = "120/80",
                Temperature = 38.2m,
                HeartRate = 85,
                Weight = 70.0m,
                VisitDate = new DateTime(2026, 2, 16, 8, 15, 0),
                CreatedAt = new DateTime(2026, 2, 16, 8, 30, 0)
            },
            new Visit
            {
                Id = visit2,
                PatientId = patient6,
                DoctorId = doctorId,
                NurseId = nurseId,
                Diagnosis = "ارتفاع ضغط الدم",
                Prescriptions = "Amlodipine 5mg - مرة واحدة يومياً صباحاً",
                Notes = "ينصح بتقليل الملح في الطعام وممارسة الرياضة",
                BloodPressure = "150/95",
                Temperature = 36.8m,
                HeartRate = 72,
                Weight = 95.5m,
                VisitDate = new DateTime(2026, 2, 15, 10, 0, 0),
                CreatedAt = new DateTime(2026, 2, 15, 10, 20, 0)
            },
            new Visit
            {
                Id = visit3,
                PatientId = patient8,
                DoctorId = doctorId,
                NurseId = nurseId,
                Diagnosis = "فحص دوري - السكري",
                Prescriptions = "Metformin 500mg - مرتين يومياً مع الوجبات",
                Notes = "مستوى السكر التراكمي مستقر، المتابعة بعد 3 أشهر",
                VisitDate = new DateTime(2026, 2, 14, 11, 0, 0),
                CreatedAt = new DateTime(2026, 2, 14, 11, 25, 0)
            }
        );

        // Seed Sample Invoices
        modelBuilder.Entity<Invoice>().HasData(
            new Invoice
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                VisitId = visit1,
                PatientId = patient1,
                Amount = 150.00m,
                TaxAmount = 22.50m, // 15% VAT
                Status = InvoiceStatus.Paid,
                PaymentMethod = PaymentMethod.Card,
                CreatedById = cashierId,
                CreatedAt = new DateTime(2026, 2, 16, 8, 35, 0),
                PaidAt = new DateTime(2026, 2, 16, 8, 40, 0)
            },
            new Invoice
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                VisitId = visit2,
                PatientId = patient6,
                Amount = 200.00m,
                TaxAmount = 30.00m,
                Status = InvoiceStatus.Pending,
                CreatedById = cashierId,
                CreatedAt = new DateTime(2026, 2, 15, 10, 25, 0)
            }
        );

        // Seed Sample Lab Tests
        modelBuilder.Entity<LabTest>().HasData(
            new LabTest
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                VisitId = visit3,
                PatientId = patient8,
                TestName = "فحص السكر التراكمي Hba1c",
                ResultNotes = "النتيجة: 6.2% - طبيعي بالنسبة لمريض سكري",
                Status = LabTestStatus.Completed,
                RequestedById = doctorId,
                PerformedById = labTechId,
                RequestedAt = new DateTime(2026, 2, 14, 11, 10, 0),
                CompletedAt = new DateTime(2026, 2, 14, 13, 0, 0)
            }
        );
    }
}
