using HealthCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCenter.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<QueueTicket> QueueTickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure entities
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<QueueTicket>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
