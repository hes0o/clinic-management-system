using System;
using System.Collections.Generic;

namespace HealthCenter.Desktop.Database.Entities;

public enum UserRole
{
    Admin,
    Doctor,
    Receptionist
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
}
