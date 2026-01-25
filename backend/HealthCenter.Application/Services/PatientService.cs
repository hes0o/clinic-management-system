using HealthCenter.Application.Interfaces;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Application.Services;

public class PatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return await _patientRepository.GetAllAsync();
    }

    public async Task<Patient?> GetPatientByIdAsync(Guid id)
    {
        return await _patientRepository.GetByIdAsync(id);
    }

    public async Task<Patient> AddPatientAsync(Patient patient)
    {
        patient.Id = Guid.NewGuid();
        return await _patientRepository.AddAsync(patient);
    }
}
