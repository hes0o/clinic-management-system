using HealthCenter.Application.Dtos;
using HealthCenter.Application.Interfaces;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientResponseDto> AddPatientAsync(CreatePatientRequestDto request)
    {
        var patient = Patient.Create(
            request.FullName,
            request.PhoneNumber,
            request.BirthDate,
            request.Gender
        );

        await _patientRepository.AddAsync(patient);

        return new PatientResponseDto(
            patient.Id,
            patient.FullName,
            patient.PhoneNumber,
            patient.BirthDate,
            patient.Gender
        );
    }

    public async Task<IReadOnlyList<PatientResponseDto>> GetAllPatientsAsync()
    {
        var patients = await _patientRepository.GetAllAsync();

        return patients.Select(p => new PatientResponseDto(
            p.Id,
            p.FullName,
            p.PhoneNumber,
            p.BirthDate,
            p.Gender
        )).ToList();
    }
}
