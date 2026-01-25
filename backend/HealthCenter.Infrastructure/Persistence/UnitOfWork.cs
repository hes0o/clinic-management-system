using HealthCenter.Application.Interfaces;

namespace HealthCenter.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IPatientRepository _patientRepository;
    private bool _disposed;

    public UnitOfWork(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public IPatientRepository Patients => _patientRepository;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In a real implementation with EF Core, this would call DbContext.SaveChangesAsync()
        // For now, with in-memory storage, we return success
        return Task.FromResult(1);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}
