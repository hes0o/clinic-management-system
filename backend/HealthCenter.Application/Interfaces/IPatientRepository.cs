using HealthCenter.Domain.Entities;

namespace HealthCenter.Application.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(Guid id);
    Task<Patient> AddAsync(Patient patient);
}
