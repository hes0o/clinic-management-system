using HealthCenter.Application.Interfaces;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Infrastructure.Repositories;

/// <summary>
/// Patient repository implementation - LSP compliant
/// Can be substituted with any IPatientRepository implementation (e.g., EF Core, Dapper)
/// </summary>
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
        if (patient == null)
            throw new ArgumentNullException(nameof(patient));

        _patients.Add(patient);
        return Task.FromResult(patient);
    }

    public Task<Patient> UpdateAsync(Patient patient)
    {
        if (patient == null)
            throw new ArgumentNullException(nameof(patient));

        var existing = _patients.FirstOrDefault(p => p.Id == patient.Id);
        if (existing == null)
            throw new InvalidOperationException("Patient not found");

        _patients.Remove(existing);
        _patients.Add(patient);
        
        return Task.FromResult(patient);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
            return Task.FromResult(false);

        _patients.Remove(patient);
        return Task.FromResult(true);
    }

    public Task<Patient?> GetByPhoneAsync(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Task.FromResult<Patient?>(null);

        var patient = _patients.FirstOrDefault(p => p.Phone.Value == phone.Trim());
        return Task.FromResult(patient);
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        var exists = _patients.Any(p => p.Id == id);
        return Task.FromResult(exists);
    }
}
