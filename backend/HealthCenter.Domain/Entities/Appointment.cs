namespace HealthCenter.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public DateTime ScheduledAt { get; private set; }

    private Appointment() { }

    public Appointment(Guid patientId, DateTime scheduledAt)
    {
        if (scheduledAt < DateTime.Now)
            throw new ArgumentException("Appointment cannot be in the past");

        Id = Guid.NewGuid();
        PatientId = patientId;
        ScheduledAt = scheduledAt;
    }
}