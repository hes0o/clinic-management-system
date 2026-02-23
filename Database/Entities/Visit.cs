using System;

using System.Collections.Generic;

namespace HealthCenter.Desktop.Database.Entities;

/// <summary>
/// Medical visit record (EMR entry)
/// </summary>
public class Visit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? NurseId { get; set; }
    public string? Diagnosis { get; set; }
    public string? Prescriptions { get; set; }
    public string? Notes { get; set; }
    
    // Vital Signs
    public string? BloodPressure { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Weight { get; set; }
    
    public string? Attachments { get; set; } // JSON array of file paths
    public DateTime VisitDate {get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Patient? Patient { get; set; }
    public User? Doctor { get; set; }
    public User? Nurse { get; set; }
    
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
}
