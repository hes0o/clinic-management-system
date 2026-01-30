using System;

namespace HealthCenter.Desktop.Database.Entities;

public enum AppointmentStatus
{
    Scheduled,
    Arrived,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

/// <summary>
/// Appointment for scheduling patient visits
/// </summary>
public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public int DurationMinutes { get; set; } = 15;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Patient? Patient { get; set; }
    public User? Doctor { get; set; }
}
