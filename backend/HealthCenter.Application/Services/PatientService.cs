using HealthCenter.Application.Common;
using HealthCenter.Application.Dtos;
using HealthCenter.Application.Interfaces;
using HealthCenter.Domain.Common;
using HealthCenter.Domain.Entities;

namespace HealthCenter.Application.Services;

/// <summary>
/// Patient service implementation - OCP, LSP, DIP compliant
/// Open for extension through dependency injection, closed for modification
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper<Patient, PatientDto> _mapper;
    private readonly IValidator<CreatePatientRequest> _validator;

    public PatientService(
        IPatientRepository patientRepository,
        IMapper<Patient, PatientDto> mapper,
        IValidator<CreatePatientRequest> validator)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Result<IEnumerable<PatientDto>>> GetAllPatientsAsync()
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync();
            var dtos = _mapper.Map(patients);
            return Result<IEnumerable<PatientDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PatientDto>>.Failure($"Failed to retrieve patients: {ex.Message}");
        }
    }

    public async Task<Result<PatientDto>> GetPatientByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return Result<PatientDto>.Failure("Invalid patient ID");

            var patient = await _patientRepository.GetByIdAsync(id);
            
            if (patient == null)
                return Result<PatientDto>.Failure("Patient not found");

            var dto = _mapper.Map(patient);
            return Result<PatientDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<PatientDto>.Failure($"Failed to retrieve patient: {ex.Message}");
        }
    }

    public async Task<Result<PatientDto>> AddPatientAsync(CreatePatientRequest request)
    {
        try
        {
            // Validate request
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsSuccess)
                return Result<PatientDto>.Failure(validationResult.Error!);

            // Check for duplicate phone
            var existingPatient = await _patientRepository.GetByPhoneAsync(request.Phone);
            if (existingPatient != null)
                return Result<PatientDto>.Failure("A patient with this phone number already exists");

            // Create domain entity using factory method
            var patient = Patient.Create(request.FullName, request.Phone);
            
            // Persist
            var created = await _patientRepository.AddAsync(patient);
            
            // Map to DTO
            var dto = _mapper.Map(created);
            return Result<PatientDto>.Success(dto);
        }
        catch (ArgumentException ex)
        {
            return Result<PatientDto>.Failure($"Validation error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<PatientDto>.Failure($"Failed to add patient: {ex.Message}");
        }
    }

    public async Task<Result<PatientDto>> UpdatePatientContactAsync(Guid id, string phone)
    {
        try
        {
            if (id == Guid.Empty)
                return Result<PatientDto>.Failure("Invalid patient ID");

            if (string.IsNullOrWhiteSpace(phone))
                return Result<PatientDto>.Failure("Phone is required");

            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return Result<PatientDto>.Failure("Patient not found");

            // Use domain method to update
            patient.UpdateContactInfo(phone);
            
            var updated = await _patientRepository.UpdateAsync(patient);
            var dto = _mapper.Map(updated);
            
            return Result<PatientDto>.Success(dto);
        }
        catch (ArgumentException ex)
        {
            return Result<PatientDto>.Failure($"Validation error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<PatientDto>.Failure($"Failed to update patient: {ex.Message}");
        }
    }

    public async Task<Result> DeletePatientAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return Result.Failure("Invalid patient ID");

            var exists = await _patientRepository.ExistsAsync(id);
            if (!exists)
                return Result.Failure("Patient not found");

            var deleted = await _patientRepository.DeleteAsync(id);
            
            return deleted 
                ? Result.Success() 
                : Result.Failure("Failed to delete patient");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete patient: {ex.Message}");
        }
    }
}
