using HealthCenter.Domain.Enums;

namespace HealthCenter.Domain.Entities;

public class QueueTicket
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public int Number { get; private set; }
    public QueueStatus Status { get; private set; }

    private QueueTicket() { }

    public QueueTicket(Guid patientId, int number)
    {
        Id = Guid.NewGuid();
        PatientId = patientId;
        Number = number;
        Status = QueueStatus.Waiting;
    }

    public void Call()
    {
        if (Status != QueueStatus.Waiting)
            throw new InvalidOperationException("Only waiting patients can be called");
// calling the queuestatus funciton to be alive 
        Status = QueueStatus.Called;
    }

    public void MarkMissed()
    {
        if (Status != QueueStatus.Called)
            throw new InvalidOperationException("Only called patients can be marked missed");

        Status = QueueStatus.Missed;
    }

    public void Complete()
    {
        if (Status != QueueStatus.Called)
            throw new InvalidOperationException("Only called patients can be completed");

        Status = QueueStatus.Completed;
    }
}