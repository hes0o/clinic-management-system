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
            entity.HasIndex(e => e.VisitDate);
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

        // Seed default admin user
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            Username = "admin",
            PasswordHash = "admin123", // TODO: Hash this properly
            FullName = "مدير النظام",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.Now
        });

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
                Id = patient1,
                FullName = "أحمد محمد علي", 
                PhoneNumber = "0501234567",
                DateOfBirth = new DateTime(1985, 5, 15),
                Gender = Gender.Male,
                Address = "الرياض، حي الملز",
                BloodType = "A+",
                EmergencyContact = "0559876543",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient2,
                FullName = "فاطمة خالد العمري", 
                PhoneNumber = "0559876543",
                DateOfBirth = new DateTime(1990, 8, 22),
                Gender = Gender.Female,
                Address = "جدة، حي الروضة",
                BloodType = "O-",
                EmergencyContact = "0501234567",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient3,
                FullName = "عبدالله سعد الغامدي", 
                PhoneNumber = "0551112233",
                DateOfBirth = new DateTime(1978, 3, 10),
                Gender = Gender.Male,
                Address = "الدمام، حي الفيصلية",
                BloodType = "B+",
                EmergencyContact = "0554445566",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient4,
                FullName = "نورة عبدالرحمن القحطاني", 
                PhoneNumber = "0544445566",
                DateOfBirth = new DateTime(1995, 12, 5),
                Gender = Gender.Female,
                Address = "مكة المكرمة، العزيزية",
                BloodType = "AB+",
                EmergencyContact = "0551112233",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient5,
                FullName = "محمد عبدالعزيز الشهري", 
                PhoneNumber = "0567778899",
                DateOfBirth = new DateTime(1982, 7, 18),
                Gender = Gender.Male,
                Address = "أبها، حي المنهل",
                BloodType = "O+",
                EmergencyContact = "0561234567",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient6,
                FullName = "سارة أحمد الدوسري", 
                PhoneNumber = "0533334444",
                DateOfBirth = new DateTime(1988, 11, 30),
                Gender = Gender.Female,
                Address = "الرياض، حي النخيل",
                BloodType = "A-",
                EmergencyContact = "0522223333",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient7,
                FullName = "خالد فهد المطيري", 
                PhoneNumber = "0522223333",
                DateOfBirth = new DateTime(1992, 4, 25),
                Gender = Gender.Male,
                Address = "الطائف، حي الشهداء",
                BloodType = "B-",
                EmergencyContact = "0533334444",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient8,
                FullName = "منى سليمان الحربي", 
                PhoneNumber = "0588889999",
                DateOfBirth = new DateTime(1975, 9, 12),
                Gender = Gender.Female,
                Address = "المدينة المنورة، حي العزيزية",
                BloodType = "O+",
                EmergencyContact = "0577778888",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient9,
                FullName = "ياسر علي الزهراني", 
                PhoneNumber = "0577778888",
                DateOfBirth = new DateTime(1987, 2, 8),
                Gender = Gender.Male,
                Address = "جدة، حي السلامة",
                BloodType = "AB-",
                EmergencyContact = "0588889999",
                CreatedAt = DateTime.Now
            },
            new Patient 
            { 
                Id = patient10,
                FullName = "ريم محمد القرني", 
                PhoneNumber = "0511112222",
                DateOfBirth = new DateTime(1993, 6, 14),
                Gender = Gender.Female,
                Address = "الخبر، حي الثقبة",
                BloodType = "A+",
                EmergencyContact = "0599998888",
                CreatedAt = DateTime.Now
            }
        );

        // Seed sample queue tickets (5 tickets for today)
        var today = DateTime.Today;
        modelBuilder.Entity<QueueTicket>().HasData(
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                TicketNumber = 1,
                PatientId = patient1,
                DoctorId = adminId,
                Status = TicketStatus.Completed,
                CreatedAt = today.AddHours(8),
                CalledAt = today.AddHours(8).AddMinutes(15),
                CompletedAt = today.AddHours(8).AddMinutes(30),
                CallCount = 1
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                TicketNumber = 2,
                PatientId = patient2,
                DoctorId = adminId,
                Status = TicketStatus.InProgress,
                CreatedAt = today.AddHours(8).AddMinutes(10),
                CalledAt = today.AddHours(8).AddMinutes(35),
                CallCount = 1
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                TicketNumber = 3,
                PatientId = patient3,
                Status = TicketStatus.Waiting,
                CreatedAt = today.AddHours(8).AddMinutes(20),
                CallCount = 0
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                TicketNumber = 4,
                PatientId = patient4,
                Status = TicketStatus.Waiting,
                CreatedAt = today.AddHours(9),
                CallCount = 0
            },
            new QueueTicket
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                TicketNumber = 5,
                PatientId = patient5,
                Status = TicketStatus.Waiting,
                CreatedAt = today.AddHours(9).AddMinutes(15),
                CallCount = 0
            }
        );

        // Seed sample visits (3 visits)
        modelBuilder.Entity<Visit>().HasData(
            new Visit
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                PatientId = patient1,
                DoctorId = adminId,
                Diagnosis = "التهاب الحلق الحاد",
                Prescriptions = "Amoxicillin 500mg - 3 مرات يومياً لمدة 5 أيام",
                Notes = "المريض يعاني من ارتفاع طفيف في درجة الحرارة",
                InvoiceAmount = 150.00m,
                VisitDate = today.AddHours(8).AddMinutes(15),
                CreatedAt = today.AddHours(8).AddMinutes(30)
            },
            new Visit
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                PatientId = patient6,
                DoctorId = adminId,
                Diagnosis = "ارتفاع ضغط الدم",
                Prescriptions = "Amlodipine 5mg - مرة واحدة يومياً صباحاً",
                Notes = "ينصح بتقليل الملح في الطعام وممارسة الرياضة",
                InvoiceAmount = 200.00m,
                VisitDate = today.AddDays(-1).AddHours(10),
                CreatedAt = today.AddDays(-1).AddHours(10).AddMinutes(20)
            },
            new Visit
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                PatientId = patient8,
                DoctorId = adminId,
                Diagnosis = "فحص دوري - السكري",
                Prescriptions = "Metformin 500mg - مرتين يومياً مع الوجبات",
                Notes = "مستوى السكر التراكمي مستقر، المتابعة بعد 3 أشهر",
                InvoiceAmount = 180.00m,
                VisitDate = today.AddDays(-2).AddHours(11),
                CreatedAt = today.AddDays(-2).AddHours(11).AddMinutes(25)
            }
        );
    }
}
