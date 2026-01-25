using HealthCenter.Application.Interfaces;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private static readonly List<Patient> _patients = new();

    public Task AddAsync(Patient patient)
    {
        _patients.Add(patient);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Patient>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Patient>>(_patients.ToList());
    }
}
