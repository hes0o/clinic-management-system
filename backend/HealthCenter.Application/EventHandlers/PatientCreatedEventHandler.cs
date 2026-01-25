using HealthCenter.Application.Common;
using HealthCenter.Domain.Events;

namespace HealthCenter.Application.EventHandlers;

/// <summary>
/// Patient created event handler - OCP compliant
/// New handlers can be added without modifying existing code
/// </summary>
public class PatientCreatedEventHandler : IDomainEventHandler<PatientCreatedEvent>
{
    public Task Handle(PatientCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Example: Send welcome email, log event, trigger notifications, etc.
        Console.WriteLine($"Patient created: {domainEvent.FullName} at {domainEvent.OccurredOn}");
        
        // Future extensions: Email service, notification service, audit logging
        return Task.CompletedTask;
    }
}
