using System;

namespace HealthCenter.Desktop.Database.Entities;

public enum TicketStatus
{
    Waiting,          // ⏳ In queue
    ReadyForDoctor,   // 👨‍⚕️ Nurse finished vitals, waiting for Doctor
    Called,           // 📢 Doctor called the patient
    Present,          // ✅ Patient responded and is present
    Absent,           // ❌ Patient did not respond to call
    AwaitingRecall,   // 🔄 Will be called again later
    InProgress,       // 🏥 With the doctor
    Completed         // ✔️ Visit finished
}

/// <summary>
/// Queue ticket for managing patient flow
/// </summary>
public class QueueTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int TicketNumber { get; set; }
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Waiting;
    public DateTime CreatedAt { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int CallCount { get; set; } = 0; // How many times patient was called

    // Navigation properties
    public Patient? Patient { get; set; }
    public User? Doctor { get; set; }
}
