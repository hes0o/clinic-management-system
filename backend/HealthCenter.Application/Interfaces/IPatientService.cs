using HealthCenter.Application.Dtos;
using HealthCenter.Domain.Common;

namespace HealthCenter.Application.Interfaces;

/// <summary>
/// Patient service interface - DIP compliant, abstracts business logic
/// </summary>
public interface IPatientService
{
    Task<Result<IEnumerable<PatientDto>>> GetAllPatientsAsync();
    Task<Result<PatientDto>> GetPatientByIdAsync(Guid id);
    Task<Result<PatientDto>> AddPatientAsync(CreatePatientRequest request);
    Task<Result<PatientDto>> UpdatePatientContactAsync(Guid id, string phone);
    Task<Result> DeletePatientAsync(Guid id);
}
