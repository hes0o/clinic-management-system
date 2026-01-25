namespace HealthCenter.Application.Common;

/// <summary>
/// Generic mapper interface - OCP compliant for future mapping strategies
/// </summary>
public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource source);
    IEnumerable<TDestination> Map(IEnumerable<TSource> sources);
}
