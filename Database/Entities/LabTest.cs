using System;

namespace HealthCenter.Desktop.Database.Entities;

public enum LabTestStatus
{
    Requested,
    InProgress,
    Completed
}

/// <summary>
/// Representation of a lab test or X-Ray requested by a Doctor and fulfilled by a Tech
/// </summary>
public class LabTest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VisitId { get; set; }
    public Guid PatientId { get; set; }

    public required string TestName { get; set; } // e.g., "CBC Blood Test", "Chest X-Ray"
    public string? ResultNotes { get; set; } // Observations from the Lab Technician
    public string? AttachmentPath { get; set; } // Local path or URL to PDF/Image of the results

    public LabTestStatus Status { get; set; } = LabTestStatus.Requested;

    public Guid RequestedById { get; set; } // Doctor who asked for the test
    public Guid? PerformedById { get; set; } // Lab Technician who uploaded the results

    public DateTime RequestedAt { get; set; } // Time requested
    public DateTime? CompletedAt { get; set; } // Time results were uploaded

    // Navigation properties
    public Visit? Visit { get; set; }
    public Patient? Patient { get; set; }
    public User? RequestedBy { get; set; }
    public User? PerformedBy { get; set; }
}
