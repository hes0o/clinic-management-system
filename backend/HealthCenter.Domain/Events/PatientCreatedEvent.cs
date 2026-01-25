using HealthCenter.Domain.Common;

namespace HealthCenter.Domain.Events;

public sealed class PatientCreatedEvent : IDomainEvent
{
    public Guid PatientId { get; }
    public string FullName { get; }
    public DateTime OccurredOn { get; }

    public PatientCreatedEvent(Guid patientId, string fullName)
    {
        PatientId = patientId;
        FullName = fullName;
        OccurredOn = DateTime.UtcNow;
    }
}
