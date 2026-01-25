namespace HealthCenter.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
