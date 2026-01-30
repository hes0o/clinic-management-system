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
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Username = "admin",
            PasswordHash = "admin123", // TODO: Hash this properly
            FullName = "مدير النظام",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.Now
        });
    }
}
