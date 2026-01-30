using System;

namespace HealthCenter.Desktop.Database.Entities;

/// <summary>
/// Medical visit record (EMR entry)
/// </summary>
public class Visit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string? Diagnosis { get; set; }
    public string? Prescriptions { get; set; }
    public string? Notes { get; set; }
    public string? Attachments { get; set; } // JSON array of file paths
    public decimal? InvoiceAmount { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.Now;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Patient? Patient { get; set; }
    public User? Doctor { get; set; }
}
