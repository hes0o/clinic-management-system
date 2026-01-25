using HealthCenter.Domain.Entities;

namespace HealthCenter.Application.Interfaces;

public interface IPatientRepository
{
    Task AddAsync(Patient patient);
    Task<IReadOnlyList<Patient>> GetAllAsync();
}
