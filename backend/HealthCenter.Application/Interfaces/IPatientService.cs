using HealthCenter.Application.Dtos;

namespace HealthCenter.Application.Interfaces;

public interface IPatientService
{
    Task<PatientResponseDto> AddPatientAsync(CreatePatientRequestDto request);
    Task<IReadOnlyList<PatientResponseDto>> GetAllPatientsAsync();
}
