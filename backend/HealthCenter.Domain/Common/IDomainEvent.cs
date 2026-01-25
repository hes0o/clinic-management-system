namespace HealthCenter.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
