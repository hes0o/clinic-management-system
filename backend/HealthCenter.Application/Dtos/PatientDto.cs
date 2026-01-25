namespace HealthCenter.Application.Dtos;

/// <summary>
/// DTOs - Immutable records following LSP
/// </summary>
public record PatientDto(Guid Id, string FullName, string Phone);

public record CreatePatientRequest(string FullName, string Phone);

public record UpdatePatientContactRequest(string Phone);
