using System;
using System.Collections.Generic;

namespace HealthCenter.Desktop.Database.Entities;

/// <summary>
/// Patient record storing personal and medical information
/// </summary>
public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? MedicalHistory { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<QueueTicket> QueueTickets { get; set; } = new List<QueueTicket>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
