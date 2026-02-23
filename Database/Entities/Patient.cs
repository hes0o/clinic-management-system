using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthCenter.Desktop.Database.Entities;

/// <summary>
/// Patient record — central identity for the entire clinic workflow
/// </summary>
public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // ── Identity ──────────────────────────────────────────
    [Required, MaxLength(100)]
    public required string FullName { get; set; }

    /// <summary>National ID (e.g. Saudi Iqama / National number) — unique identifier</summary>
    [MaxLength(20)]
    public string? NationalId { get; set; }

    [Required, Phone]
    public required string PhoneNumber { get; set; }

    public string? EmergencyContact { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    // ── Medical Profile ───────────────────────────────────
    [MaxLength(5)]
    public string? BloodType { get; set; }

    /// <summary>Known allergies (e.g. "Penicillin, Dust")</summary>
    public string? Allergies { get; set; }

    /// <summary>Chronic conditions (e.g. "Diabetes Type 2, Hypertension")</summary>
    public string? ChronicConditions { get; set; }

    /// <summary>Freeform medical history notes</summary>
    public string? MedicalHistory { get; set; }

    // ── Insurance ─────────────────────────────────────────
    public string? InsuranceProvider { get; set; }

    public string? InsuranceNumber { get; set; }

    // ── Timestamps ────────────────────────────────────────
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ── Navigation ────────────────────────────────────────
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<QueueTicket> QueueTickets { get; set; } = new List<QueueTicket>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}

public enum Gender
{
    Male,
    Female
}
