using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthCenter.Desktop.Database.Entities;

/// <summary>
/// Patient record storing personal and medical information
/// </summary>
public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public required string FullName { get; set; }
    
    [Required]
    [Phone]
    public required string PhoneNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public Gender? Gender { get; set; }
    
    [MaxLength(200)]
    public string? Address { get; set; }
    
    [MaxLength(5)]
    public string? BloodType { get; set; }
    
    [Phone]
    public string? EmergencyContact { get; set; }
    
    public string? MedicalHistory { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<QueueTicket> QueueTickets { get; set; } = new List<QueueTicket>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}

/// <summary>
/// Gender enumeration
/// </summary>
public enum Gender
{
    Male,    // ذكر
    Female   // أنثى
}
