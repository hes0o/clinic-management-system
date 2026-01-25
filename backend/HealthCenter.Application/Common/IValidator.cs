using HealthCenter.Domain.Common;

namespace HealthCenter.Application.Common;

/// <summary>
/// Generic validator interface - OCP compliant for validation strategies
/// </summary>
public interface IValidator<T>
{
    Result Validate(T item);
}
