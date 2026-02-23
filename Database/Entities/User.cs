using System;
using System.Collections.Generic;

namespace HealthCenter.Desktop.Database.Entities;

public enum UserRole
{
    SuperAdmin,       // Full system access, staff management
    ClinicManager,    // Financials and reporting
    Doctor,           // Medical records, prescriptions
    Nurse,            // Vital signs, intake
    Receptionist,     // Patient registration, scheduling, queue
    Cashier,          // Invoicing, insurance verification
    LabTechnician     // Uploading test results
}

/// <summary>
/// System user (doctors, receptionists, admins)
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public UserRole Role { get; set; } = UserRole.Receptionist;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<QueueTicket> QueueTickets { get; set; } = new List<QueueTicket>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<LabTest> RequestedLabTests { get; set; } = new List<LabTest>();
    public ICollection<LabTest> PerformedLabTests { get; set; } = new List<LabTest>();
}
