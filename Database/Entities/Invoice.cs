using System;

namespace HealthCenter.Desktop.Database.Entities;

public enum PaymentMethod
{
    Cash,
    Card,
    Insurance
}

public enum InvoiceStatus
{
    Pending,
    Paid,
    Voided
}

/// <summary>
/// Billing and financial record for a patient visit
/// </summary>
public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VisitId { get; set; }
    public Guid PatientId { get; set; }
    
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public PaymentMethod? PaymentMethod { get; set; }
    
    public Guid CreatedById { get; set; } // The Cashier who generated/handled it
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    // Navigation properties
    public Visit? Visit { get; set; }
    public Patient? Patient { get; set; }
    public User? CreatedBy { get; set; }
}
