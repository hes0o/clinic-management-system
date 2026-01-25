using HealthCenter.Application.Interfaces;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private static readonly List<Patient> _patients = new();

    public Task<IEnumerable<Patient>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Patient>>(_patients);
    }

    public Task<Patient?> GetByIdAsync(Guid id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(patient);
    }

    public Task<Patient> AddAsync(Patient patient)
    {
        _patients.Add(patient);
        return Task.FromResult(patient);
    }
}
