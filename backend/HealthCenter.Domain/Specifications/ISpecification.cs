using System.Linq.Expressions;
using HealthCenter.Domain.Common;

namespace HealthCenter.Domain.Specifications;

/// <summary>
/// Specification pattern - OCP compliant for query logic
/// Allows adding new query specifications without modifying existing code
/// </summary>
public interface ISpecification<T> where T : Entity
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}
