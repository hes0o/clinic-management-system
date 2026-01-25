using HealthCenter.Domain.Common;

namespace HealthCenter.Application.Common;

/// <summary>
/// Domain event handler interface - OCP compliant for event-driven architecture
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);
}
